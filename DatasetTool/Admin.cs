/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-6-23
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core.Services.Security;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Security;
using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Core.Model;
using System.Linq.Expressions;
using OpenIZ.Core;
using System.Diagnostics.Tracing;
using static OizDevTool.Debugger.BreDebugger;
using OpenIZ.Core.Diagnostics;
using System.Diagnostics;
using OpenIZ.Core.Security.Attribute;
using OpenIZ.Core.Model.Security;

namespace OizDevTool
{
    /// <summary>
    /// Represents administrative functions
    /// </summary>
    [Description("Administrative functions for creating users, groups, etc.")]
    public static class Admin
    {

        /// <summary>
        /// Administrative shell
        /// </summary>
        public class AdminShell : InteractiveBase
        {

            public AdminShell()
            {
                AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.AnonymousPrincipal);
                ApplicationServiceContext.Current = ApplicationContext.Current;
                ApplicationContext.Current.AddServiceProvider(typeof(ServiceManager));
                System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.ConsoleTraceListener()
                {
                    Filter = new EventTypeFilter(SourceLevels.Warning | SourceLevels.Error)
                });

                ApplicationContext.Current.Start();

                Tracer.AddWriter(new ConsoleTraceWriter(EventLevel.LogAlways, "dbg"), EventLevel.LogAlways);
                Console.Clear();
                Console.WriteLine("Open Administration Console v{0}", typeof(Program).Assembly.GetName().Version);
                Console.WriteLine("Copyright (C) 2015 - 2017, Mohawk College of Applied Arts and Technology");

                this.m_prompt = "> ";
            }

            #region Helpers

            // Primitives
            private static readonly HashSet<Type> primitives = new HashSet<Type>()
            {
                typeof(bool),
                typeof(bool?),
                typeof(int),
                typeof(int?),
                typeof(float),
                typeof(float?),
                typeof(double),
                typeof(double?),
                typeof(decimal),
                typeof(decimal?),
                typeof(String),
                typeof(Guid),
                typeof(Guid?),
                typeof(Type),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(UInt32),
                typeof(UInt32?),
                typeof(byte[])
            };


            /// <summary>
            /// Get scope object
            /// </summary>
            protected object GetScopeObject(object obj, string path)
            {
                var pathPart = path?.Split('.');
                if (pathPart?.Length > 0)
                {
                    Console.WriteLine("Path: {0}", path);
                    foreach (var pp in pathPart)
                    {
                        var p = pp;
                        if (p.Contains("["))
                            p = p.Substring(0, p.IndexOf("["));

                        if (!String.IsNullOrEmpty(p))
                            obj = obj?.GetType().GetProperty(p).GetValue(obj);
                        p = pp;
                        while (p.Contains("[") && p.Contains("]") && (obj is IList || obj is IDictionary))
                        {
                            var idx = p.Substring(p.IndexOf("[") + 1, p.IndexOf("]") - p.IndexOf("[") - 1);
                            if (obj is IDictionary)
                            {

                                var key = (obj as IDictionary).Keys.OfType<Object>().FirstOrDefault(o => o.ToString() == idx);
                                obj = (obj as IDictionary)[key ?? idx];
                            }
                            else
                                obj = (obj as IList)[Int32.Parse(idx)];
                            p = p.Substring(p.IndexOf("]") + 1);

                            if (obj is ExpandoObject)
                                obj = new Dictionary<String, Object>(obj as ExpandoObject);
                        }
                    }
                }
                return obj;
            }


            /// <summary>
            /// Dump object to screen
            /// </summary>
            protected void DumpObject(object obj, String path)
            {
                if (obj == null)
                    Console.WriteLine("null");
                else
                {

                    if (obj is System.Dynamic.ExpandoObject)
                        obj = new Dictionary<String, Object>(obj as System.Dynamic.ExpandoObject);

                    obj = this.GetScopeObject(obj, path);

                    int maxWidth = (Console.WindowWidth / 6);

                    if (obj is IList)
                    {
                        int i = 0;
                        Console.WriteLine("Count:{0}", (obj as IList).Count);
                        foreach (var itm in obj as IList)
                        {
                            Console.WriteLine("[{0}] {1}", i, (obj as IList)[i++]);
                            if (i > 99) break;
                        }
                    }
                    else if (obj is IDictionary)
                    {
                        Console.WriteLine("Count:{0}", (obj as IDictionary).Count);
                        int i = 0;
                        foreach (var itm in (obj as IDictionary))
                        {
                            var k = (obj as IDictionary).Keys.OfType<Object>().Skip(i++).First();
                            Console.WriteLine("[{0}] {1}", k, (obj as IDictionary)[k]);

                            if (i > 99) break;
                        }
                    }
                    else if (primitives.Contains(obj.GetType()))
                        Console.WriteLine(obj);
                    else if (obj is IEnumerable)
                    {
                        foreach (var itm in (obj as IEnumerable))
                        {
                            Console.WriteLine("{0}", itm);
                        }
                    }
                    else
                    {
                        foreach (var itm in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(o => o.Name))
                        {
                            String nameStr = itm.Name,
                                typeStr = itm.PropertyType.Name,
                                valStr = itm.GetValue(obj)?.ToString();

                            if (nameStr.Length > maxWidth - 4)
                                nameStr = nameStr.Substring(0, maxWidth - 4) + "...";
                            if (typeStr.Length > maxWidth - 4)
                                typeStr = typeStr.Substring(0, maxWidth - 4) + "...";
                            if (valStr?.Length > (maxWidth * 4))
                                valStr = valStr.Substring(0, (maxWidth * 4) - 4) + "...";

                            Console.WriteLine("{0}{1}{2}{3}{4}",
                                nameStr, new String(' ', maxWidth - nameStr.Length),
                                typeStr, new String(' ', maxWidth - typeStr.Length),
                                valStr == String.Empty ? "\"\"" : valStr ?? "null");
                        }
                    }
                }
            }


            #endregion


            /// <summary>
            /// Authenticate the session
            /// </summary>
            [Command("auth", "Sets the current session to the specified user")]
            public void Authenticate(string userName, string password)
            {
                var principal = ApplicationContext.Current.GetService<IIdentityProviderService>()?.Authenticate(userName, password);
                if (principal == null)
                    throw new SecurityException("Could not authenticate. Is IIdentityProviderService registered?");
                AuthenticationContext.Current = new AuthenticationContext(principal);

                if (ApplicationContext.Current.GetService<IRoleProviderService>().IsUserInRole(userName, "ADMINISTRATORS"))
                    this.m_prompt = "# ";
                else
                    this.m_prompt = "> ";
            }

            /// <summary>
            /// Query for the specified type
            /// </summary>
            public void Query(string type, string query)
            {
                this.Query(type, query, "0", "100", null);
            }

            /// <summary>
            /// Queries the IMS clinical store for the specified data
            /// </summary>
            [Command("query", "Queries the IMS clinical store for the specified item with specified limits")]
            public void Query(string type, string query, string skip, string take, string path)
            {
                var t = Type.GetType(type);
                if (t == null)
                    t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, type);
                if (t == null)
                {
                    t = typeof(Int32).Assembly.ExportedTypes.FirstOrDefault(tr => tr.Namespace == "System" && tr.Name == type);
                    if (t == null)
                        throw new MissingMethodException(type, (string)null);
                }

                var idp = typeof(IDataPersistenceService<>).MakeGenericType(t);
                var ids = ApplicationContext.Current.GetService(idp) as IDataPersistenceService;
                if (ids == null)
                    throw new InvalidOperationException($"Persistence service for {type} not found");

                var mi = typeof(QueryExpressionParser).GetGenericMethod("BuildLinqExpression", new Type[] { t }, new Type[] { typeof(NameValueCollection) });
                var qd = mi.Invoke(null, new object[] { NameValueCollection.ParseQueryString(query) }) as Expression;

                int? o = String.IsNullOrEmpty(skip) ? 0 : Int32.Parse(skip),
                    c = String.IsNullOrEmpty(take) ? (int?)null : Int32.Parse(take);

                Console.WriteLine("INF: {0} where {1} ({2}..{3})", type, qd, o, c);
                int tc = 0;
                var retVal = ids.Query(qd, o.Value, c, out tc);

                this.DumpObject(retVal, path);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("lock", "Locks the specified user")]
            public void LockUser(string userName)
            {
                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var user = securityService.GetUser(userName);
                securityService.LockUser(user.Key.Value);
            }

            /// <summary>
            /// User stat dump
            /// </summary>
            [Command("userlist", "Lists details about the specified users")]
            public void UserStat()
            {
                this.UserStat(String.Empty);
            }
            /// <summary>
            /// User stat dump
            /// </summary>
            [Command("userlist", "Lists details about the specified users")]
            public void UserStat(string userName)
            {

                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var users = securityService.FindUsers(o=>o.UserName.Contains(userName));
                Console.WriteLine("SID{0}UserName{1}Last Lgn{2}Lockout{2} ILA  A", new String(' ', 37), new String(' ', 32), new String(' ', 13));
                foreach(var usr in users)
                {
                    Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{5}{7}{8}{9}",
                        usr.Key.Value.ToString("B"),
                        new String(' ', 2),
                        usr.UserName.Length > 38 ? usr.UserName.Substring(0, 38) : usr.UserName,
                        new String(' ', usr.UserName.Length > 38 ? 2 : 40 - usr.UserName.Length),
                        usr.LastLoginTime.HasValue ? usr.LastLoginTime?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") : new String(' ', 19),
                        "  ",
                        usr.Lockout.HasValue ? usr.Lockout?.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss") : new string(' ', 19),
                        usr.InvalidLoginAttempts,
                        new String(' ', 4 - usr.InvalidLoginAttempts.ToString().Length ),
                        usr.ObsoletionTime.HasValue ? "  " : " *"
                        );
                }
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("unlock", "Unlocks the specified user")]
            public void UnlockUser(string userName)
            {
                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var user = securityService.GetUser(userName);
                securityService.UnlockUser(user.Key.Value);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("passwd", "Sets a user's password")]
            public void SetPassword(string userName, string password)
            {
                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                var user = securityService.GetUser(userName);
                securityService.ChangePassword(user.Key.Value, password);
            }

            /// <summary>
            /// Add a user
            /// </summary>
            /// <param name="userName"></param>
            /// <param name="password"></param>
            [Command("useradd", "Adds a user to OpenIZ")]
            public void UserAdd(string userName, string password)
            {
                var securityService = ApplicationContext.Current.GetService<ISecurityRepositoryService>();
                securityService.CreateUser(new SecurityUser()
                {
                    UserName = userName
                }, password);
            }

            /// <summary>
            /// Execute the tool
            /// </summary>
            [Command("exec", "Executes the specified tool")]
            public void Execute(string tool, string op)
            {
                this.Execute(tool, op, string.Empty);
            }

            /// <summary>
            /// Execute the tool
            /// </summary>
            [Command("exec", "Executes the specified tool")]
            public void Execute(string tool, string op, string parm)
            {
                var toolType = typeof(Program).Assembly.ExportedTypes.FirstOrDefault(o => o.Name == tool);
                if (toolType == null)
                    Console.WriteLine("Could not find tool: {0}", tool);
                var operation = toolType.GetMethod(op);
                if (operation == null)
                    Console.WriteLine("Tool {0} does not have operation named {1}", tool, op);

                
                String[] args = !String.IsNullOrEmpty(parm) ? parm.Split(' ') : new string[0];
                operation.Invoke(null, new object[] { args });
            }
        }

        /// <summary>
        /// Business rule
        /// </summary>
        [Description("Drops into the administrative shell")]
        [Interactive]
        public static void Shell(String[] args)
        {
            AdminShell shell = new AdminShell();
            shell.Exec();
        }

    }
}
