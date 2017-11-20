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
 * Date: 2017-6-10
 */
using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenIZ.Core.Applets.ViewModel.Json;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.EntityLoader;
using OpenIZ.Core.Model.Map;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace OizDevTool.Debugger
{
    /// <summary>
    /// Debugger base
    /// </summary>
    public abstract class DebuggerBase : InteractiveBase
    {
        // Exit debugger
        private bool m_exitRequested = false;

    
        private bool m_fullStack = false;


        /// <summary>
        /// Breakpoints
        /// </summary>
        public List<int> Breakpoints { get; set; }

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

        protected string m_workingDirectory;
        private ConsoleColor m_promptColor = Console.ForegroundColor;

        /// <summary>
        /// Sets the root path
        /// </summary>
        public DebuggerBase(string workingDir)
        {
            this.m_prompt = "dbg >";
            this.m_workingDirectory = workingDir ?? Environment.CurrentDirectory;
            this.Breakpoints = new List<int>();
        }


        /// <summary>
        /// Print stack
        /// </summary>
        protected override void PrintStack(Exception e)
        {
            if (!this.m_fullStack)
            {
                Console.Error.WriteLine("ERR: {0}", e.Message);
                var i = e.InnerException; int l = 1;
                while (i != null)
                {
                    Console.WriteLine("\t{0}:{1}", l++, i.Message);
                    i = i.InnerException;
                }
            }
            else
                Console.WriteLine(e.ToString());
        }

        /// <summary>
        /// Perform debugger
        /// </summary>
        public void Debug()
        {

            Console.CursorVisible = true;
            EntitySource.Current = new EntitySource(new PersistenceServiceEntitySource());
            Console.WriteLine("Working Directory: {0}", this.m_workingDirectory);
            base.Exec();
          
        }

        /// <summary>
        /// Output full stack
        /// </summary>
        [Command("ofs", "Enables printing of full stack traces")]
        public void OutputFullStack()
        {
            this.m_fullStack = !this.m_fullStack;
            Console.WriteLine("Full Stack: {0}", this.m_fullStack);
        }

        /// <summary>
        /// Print working directory
        /// </summary>
        [Command("pwd", "Prints the current working directory")]
        public void PrintWorkingDirectory()
        {
            Console.WriteLine(this.m_workingDirectory);
        }

        /// <summary>
        /// Print data query
        /// </summary>
        [Command("pdq", "Prints the result of a data query")]
        public void PrintDataQuery(String type, String qry)
        {
            this.PrintDataQuery(type, qry, null, null);
        }

        /// <summary>
        /// Print data query
        /// </summary>
        [Command("pdq", "Prints the result of a data query")]
        public void PrintDataQuery(String type, String qry, String skip, String take)
        {
            this.PrintDataQuery(type, qry, skip, take, null);
        }

        /// <summary>
        /// Print data query
        /// </summary>
        [Command("pdq", "Prints the result of a data query")]
        public void PrintDataQuery(String type, String qry, String path)
        {
            this.PrintDataQuery(type, qry, null, null, path);
        }
        /// <summary>
        /// Print data query
        /// </summary>
        [Command("pdq", "Prints the result of a data query")]
        public void PrintDataQuery(String type, String qry, String skip, String take, String path)
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
            var qd = mi.Invoke(null, new object[] { NameValueCollection.ParseQueryString(qry) }) as Expression;

            int? o = String.IsNullOrEmpty(skip) ? 0 : Int32.Parse(skip),
                c = String.IsNullOrEmpty(take) ? (int?)null : Int32.Parse(take);

            Console.WriteLine("INF: {0} where {1} ({2}..{3})", type, qd, o, c);
            int tc = 0;
            var retVal = ids.Query(qd, o.Value, c, out tc);

            this.DumpObject(retVal, path);
        }

        /// <summary>
        /// Changes working directory
        /// </summary>
        [Command("cd", "Changes the working directory")]
        public void ChangeWorkingDirectory(String dir)
        {
            if (dir == "..")
                dir = Path.GetDirectoryName(this.m_workingDirectory);

            if (!Path.IsPathRooted(dir))
                dir = Path.Combine(this.m_workingDirectory, dir);
            if (Directory.Exists(dir))
            {
                this.m_workingDirectory = dir;
                Console.WriteLine("INF: {0}", this.m_workingDirectory);
            }
            else
                throw new InvalidOperationException($"Directory {dir} not found");
        }

        /// <summary>
        /// Changes working directory
        /// </summary>
        [Command("ls", "List files in the working directory")]
        public void ListWorkingDirectory()
        {
            this.ListWorkingDirectory(null);
        }

        /// <summary>
        /// List working directory
        /// </summary>
        [Command("ls", "List files in the directory relative to the working directory")]
        public void ListWorkingDirectory(String dir)
        {

            if (dir != null)
            {
                if (!Path.IsPathRooted(dir))
                    dir = Path.Combine(this.m_workingDirectory, dir);
                if (!Directory.Exists(dir))
                    throw new InvalidOperationException($"Directory {dir} not found");
            }
            else
                dir = this.m_workingDirectory;

            Console.WriteLine("INF: {0}", dir);

            foreach (var d in Directory.GetDirectories(dir))
            {
                var disp = Path.GetFileName(d);
                if (disp.Length > 35)
                    disp = disp.Substring(0, 32) + "...";
                Console.WriteLine("{0}{1}[DIRECTORY]", disp, new String(' ', 35 - disp.Length));
            }

            foreach (var f in Directory.GetFiles(dir))
            {
                var disp = Path.GetFileName(f);
                if (disp.Length > 35)
                    disp = disp.Substring(0, 32) + "...";

                var fi = new FileInfo(f);
                Console.WriteLine("{0}{1}{2:###,###} b\t{3:yyyy-MMM-dd}", disp, new String(' ', 35 - disp.Length), fi.Length, fi.LastWriteTime);
            }
        }

        /// <summary>
        /// Set breakpoint
        /// </summary>
        /// <param name="ln"></param>
        [Command("b", "Sets a breakpoint on the specified line")]
        public void BreakpointSet(String line)
        {
            this.Breakpoints.Add(Int32.Parse(line));
        }

        /// <summary>
        /// Set breakpoint
        /// </summary>
        [Command("bl", "Lists all set breakpoints")]
        public void BreakpointList()
        {
            foreach (var itm in this.Breakpoints)
                Console.WriteLine(itm);
        }

        /// <summary>
        /// Set breakpoint
        /// </summary>
        [Command("bd", "Deletes the breakpoint at the specified line")]
        public void BreakpointDelete(String line)
        {
            this.Breakpoints.Remove(Int32.Parse(line));
        }

        /// <summary>
        /// Set breakpoint
        /// </summary>
        [Command("bc", "Clears all breakpoints")]
        public void BreakpointClear()
        {
            this.Breakpoints.Clear();
        }


        /// <summary>
        /// Set breakpoint
        /// </summary>
        [Command("ss", "Sets the current scope of the debugger")]
        public virtual object SetScope(String scope)
        {
            return scope;
        }

        /// <summary>
        /// Set breakpoint
        /// </summary>
        [Command("ss", "Sets the current scope of the debugger to the value")]
        public virtual object SetScope(String type, String data)
        {
            var t = Type.GetType(type);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, type);
            if (t == null)
            {
                t = typeof(Int32).Assembly.ExportedTypes.FirstOrDefault(o => o.Namespace == "System" && o.Name == type);
                if (t == null)
                    throw new MissingMethodException(type, (string)null);
                object res = null;
                if (!MapUtil.TryConvert(data, t, out res))
                    throw new ArgumentOutOfRangeException(nameof(data));
                return res;
            }
            else
            {
                return JsonConvert.DeserializeObject(data, t, new JsonSerializerSettings()
                {
                    Binder = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder(),
                    TypeNameAssemblyFormat = 0,
                    TypeNameHandling = TypeNameHandling.All
                });
            }
        }

        /// <summary>
        /// Set scope json
        /// </summary>
        [Command("sj", "Sets the current scope to an anonymous type from json")]
        public object SetScopeJson(String json)
        {
            return JsonConvert.DeserializeObject(json, new JsonSerializerSettings()
            {
                Binder = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder(),
                TypeNameAssemblyFormat = 0,
                TypeNameHandling = TypeNameHandling.All
            });
        }

        /// <summary>
        /// Set scope json
        /// </summary>
        [Command("sfv", "Sets the current scope the contents of a JSON view model file")]
        public object SetScopeJsonFile(String type, String file)
        {

            var filePath = file;
            if (!Path.IsPathRooted(filePath))
                filePath = Path.Combine(this.m_workingDirectory, file);

            var t = Type.GetType(type);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, type);
            if (t == null)
            {
                t = typeof(Int32).Assembly.ExportedTypes.FirstOrDefault(o => o.Namespace == "System" && o.Name == type);
                if (t == null)
                    throw new MissingMethodException(type, (string)null);
            }

            using (var str = File.OpenText(file))
                return new JsonViewModelSerializer().DeSerialize(str, t);

        }
        /// <summary>
        /// Set scope json
        /// </summary>
        [Command("sfj", "Sets the current scope the contents of a JSON file")]
        public object SetScopeJsonFile(String file)
        {
            var filePath = file;
            if (!Path.IsPathRooted(filePath))
                filePath = Path.Combine(this.m_workingDirectory, file);

            return JsonConvert.DeserializeObject(File.ReadAllText(filePath), new JsonSerializerSettings()
            {
                Binder = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder(),
                TypeNameAssemblyFormat = 0,
                TypeNameHandling = TypeNameHandling.All
            });
        }

        /// <summary>
        /// Set scope json
        /// </summary>
        [Command("sfx", "Sets the current scope the contents of a XML file")]
        public object SetScopeXmlFile(String type, String file)
        {
            var filePath = file;
            if (!Path.IsPathRooted(filePath))
                filePath = Path.Combine(this.m_workingDirectory, file);

            var t = Type.GetType(type);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, type);
            if (t == null)
            {
                t = typeof(Int32).Assembly.ExportedTypes.FirstOrDefault(o => o.Namespace == "System" && o.Name == type);
                if (t == null)
                    throw new MissingMethodException(type, (string)null);
            }

            XmlSerializer xsz = new XmlSerializer(t);
            using (var fs = File.OpenRead(filePath))
                return xsz.Deserialize(fs);

        }

        /// <summary>
        /// Set scope to null
        /// </summary>
        [Command("sn", "Sets the scope to null")]
        public object ClearScope()
        {
            return null;
        }


        /// <summary>
        /// Clear screen
        /// </summary>
        [Command("clear", "Clears the screen")]
        public void ClearScreen()
        {
            Console.Clear();
        }

        /// <summary>
        /// Dump scope variable to screen
        /// </summary>
        [Command("ps", "Prints scope to the screen")]
        public void PrintScope()
        {
            if (this.m_scopeObject == null)
                Console.WriteLine("null");
            else
            {
                Console.WriteLine("Type: {0}", this.m_scopeObject.GetType().FullName);
                Console.WriteLine("Asm:  {0}", this.m_scopeObject.GetType().Assembly.FullName);
                Console.WriteLine("Str:  {0}", this.m_scopeObject);
            }
        }

        /// <summary>
        /// Dump scope variable to screen
        /// </summary>
        [Command("ds", "Dumps scope to the screen")]
        public void DumpScope()
        {
            this.DumpScope(null);
        }

        /// <summary>
        /// Exchange scope
        /// </summary>
        [Command("xs", "Exchanges the current scope for the vaue of a property on another scope")]
        public object ExchangeScope(String path)
        {
            return this.GetScopeObject(this.m_scopeObject, path);
        }

        /// <summary>
        /// Dump scope variable to screen
        /// </summary>
        [Command("ds", "Dumps specified scope's path to the screen ")]
        public void DumpScope(String path)
        {
            this.PrintScope();
            this.DumpObject(this.m_scopeObject, path);
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

        /// <summary>
        /// Dump scope as json
        /// </summary>
        /// <param name="path"></param>
        [Command("dj", "Dumps scope as JSON to screen")]
        public void DumpScopeJson()
        {
            this.DumpScopeJson(null);
        }

        /// <summary>
        /// Dump scope
        /// </summary>
        /// <param name="path"></param>
        [Command("dj", "Dumps scope as JSON to screen")]
        public void DumpScopeJson(String path)
        {
            var obj = this.GetScopeObject(this.m_scopeObject, path);
            Console.WriteLine(JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = { new StringEnumConverter() }
            }));
        }

        /// <summary>
        /// Dump scope as json
        /// </summary>
        /// <param name="path"></param>
        [Command("dx", "Dumps scope as XML to screen")]
        public void DumpScopeXml()
        {
            this.DumpScopeXml(null);
        }

        /// <summary>
        /// Dump scope
        /// </summary>
        /// <param name="path"></param>
        [Command("dx", "Dumps scope as XML to screen")]
        public void DumpScopeXml(String path)
        {
            var obj = this.GetScopeObject(this.m_scopeObject, path);
            XmlSerializer xsz = new XmlSerializer(obj.GetType());
            using (var xw = XmlWriter.Create(Console.Out, new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true
            }))
                xsz.Serialize(xw, obj);
            Console.WriteLine();
        }

        /// <summary>
        /// Dump scope
        /// </summary>
        /// <param name="path"></param>
        [Command("dv", "Dumps scope as ViewModel (mobile) to screen")]
        public void DumpScopeView()
        {
            this.DumpScopeView(null);
        }

         /// <summary>
        /// Dump scope
        /// </summary>
        /// <param name="path"></param>
        [Command("dv", "Dumps scope as ViewModel (mobile) to screen")]
        public void DumpScopeView(String path)
        {
            var obj = this.GetScopeObject(this.m_scopeObject, path);
            JsonViewModelSerializer xsz = new JsonViewModelSerializer();
            using (var jv = new JsonTextWriter(Console.Out) { Formatting = Newtonsoft.Json.Formatting.Indented })
            {
                xsz.Serialize(jv, obj as IdentifiedData);
            }
            Console.WriteLine();
        }
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
        /// List all services 
        /// </summary>
        [Command("lds", "Lists all services available to the debugger")]
        public void ListServiceInfo()
        {
            this.ListServiceInfo(null);
        }

        /// <summary>
        /// Lists all services available to the debugger
        /// </summary>
        [Command("lds", "Lists all services available to the debugger")]
        public void ListServiceInfo(String name)
        {
            Console.WriteLine("Context ID: {0}", ApplicationContext.Current.ContextId);
            Console.WriteLine("IsRunning:  {0}", ApplicationContext.Current.IsRunning);
            foreach (var itm in ApplicationContext.Current.GetServices().Distinct())
            {
                if (name == null || name == itm.GetType().Name ||
                    itm.GetType().GetInterfaces().Any(o => o.Name == name))
                {
                    var svcDisplay = itm.GetType().Name;
                    if (svcDisplay.Length > 20)
                        svcDisplay = svcDisplay.Substring(0, 17) + "...";
                    var services = String.Join(",", itm.GetType().GetInterfaces().Where(o => o.Namespace.StartsWith("OpenIZ") || o.Namespace.StartsWith("MARC")).Select(o => o.Name));
                    if (name == null && services.Length > Console.WindowWidth - 20)
                        services = services.Substring(0, Console.WindowWidth - 24) + "...";

                    Console.WriteLine("{0}{1}{2}", svcDisplay, new String(' ', 20 - svcDisplay.Length), services);
                }
            }
        }


        /// <summary>
        /// Load specified data as scope
        /// </summary>
        [Command("sd", "Loads specified data type with id from the database into the scope variable")]
        public Object SetScopeDatabase(String type, String id)
        {
            var t = Type.GetType(type);
            if (t == null)
                t = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder().BindToType(typeof(IdentifiedData).Assembly.FullName, type);
            if (t == null)
            {
                t = typeof(Int32).Assembly.ExportedTypes.FirstOrDefault(o => o.Namespace == "System" && o.Name == type);
                if (t == null)
                    throw new MissingMethodException(type, (string)null);
            }

            var idp = typeof(IDataPersistenceService<>).MakeGenericType(t);
            var ids = ApplicationContext.Current.GetService(idp) as IDataPersistenceService;
            if (ids == null)
                throw new InvalidOperationException($"Persistence service for {type} not found");

            Guid gd = Guid.Parse(id);
            return ids.Get(gd);
        }

        /// <summary>
        /// Inserts the current scope item to the database
        /// </summary>
        [Command("dbi", "Inserts the current scoped object in the store")]
        public Object InsertScopeDb()
        {
            var idp = typeof(IDataPersistenceService<>).MakeGenericType(this.m_scopeObject.GetType());
            var ids = ApplicationContext.Current.GetService(idp) as IDataPersistenceService;

            if (ids == null)
                throw new InvalidOperationException($"Cannot find persister {idp}");

            return ids?.Insert(this.m_scopeObject);
        }

        /// <summary>
        /// Load specified data as scope
        /// </summary>
        [Command("sdq", "Sets scope to the result of the specified database query")]
        public Object SetScopeDatabaseQuery(String type, String qry)
        {
            return this.SetScopeDatabaseQuery(type, qry, null, null);
        }

        /// <summary>
        /// Load specified data as scope
        /// </summary>
        [Command("sdq", "Sets scope to the result of the specified database query")]
        public Object SetScopeDatabaseQuery(String type, String qry, String skip, String take)
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
            var qd = mi.Invoke(null, new object[] { NameValueCollection.ParseQueryString(qry) }) as Expression;

            int? o = String.IsNullOrEmpty(skip) ? 0 : Int32.Parse(skip),
                c = String.IsNullOrEmpty(take) ? (int?)null : Int32.Parse(take);

            Console.WriteLine("INF: {0} where {1} ({2}..{3})", type, qd, o, c);
            int tc = 0;
            var retVal = ids.Query(qd, o.Value, c, out tc);
            Console.WriteLine("INF: {0} set to scope", tc);
            return retVal;
        }


    }
}