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
using OpenIZ.AdminConsole.Attributes;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.AdminConsole.Shell
{
    /// <summary>
    /// Represents a base class for interactive shells
    /// </summary>
    [AdminCommandlet]
    public class InteractiveShell
    {
        // Exit debugger
        private bool m_exitRequested = false;

        protected string m_prompt = "> ";
        private ConsoleColor m_promptColor = Console.ForegroundColor;
        // Commandlets
        private Dictionary<String, MethodInfo> m_commandlets = new Dictionary<string, MethodInfo>();

        /// <summary>
        /// Sets the root path
        /// </summary>
        public InteractiveShell()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                foreach (var t in asm.GetTypes().Where(o => o.GetCustomAttribute<AdminCommandletAttribute>() != null))
                {
                    foreach (var me in t.GetRuntimeMethods().Where(o => o.GetCustomAttribute<AdminCommandAttribute>() != null))
                        this.m_commandlets.Add(me.GetCustomAttribute<AdminCommandAttribute>().Command, me);
                    if (t.GetRuntimeMethod("Init", new Type[0]) != null)
                        t.GetRuntimeMethod("Init", new Type[0]).Invoke(null, null);
                }
        }

        /// <summary>
        /// Get response color
        /// </summary>
        protected ConsoleColor GetResponseColor()
        {
            if (Console.BackgroundColor == ConsoleColor.Black)
                return Console.ForegroundColor != ConsoleColor.Cyan ? ConsoleColor.Cyan : ConsoleColor.Magenta;
            if (Console.BackgroundColor == ConsoleColor.Blue)
                return Console.ForegroundColor != ConsoleColor.White ? ConsoleColor.Yellow : ConsoleColor.White;
            else
                return Console.ForegroundColor != ConsoleColor.Blue ? ConsoleColor.Blue : ConsoleColor.Red;
        }

        /// <summary>
        /// Prompt
        /// </summary>
        protected void Prompt()
        {
            Console.ForegroundColor = m_promptColor;
            Console.Write(this.m_prompt);
        }

        /// <summary>
        /// Print stack
        /// </summary>
        protected virtual void PrintStack(Exception e)
        {
            Console.Error.WriteLine("ERR: {0}", e.Message);
            var i = e.InnerException; int l = 1;
            while (i != null)
            {
                Console.WriteLine("\t{0}:{1}", l++, i.Message);
                i = i.InnerException;
            }
        }


        /// <summary>
        /// Perform debugger
        /// </summary>
        public void Exec()
        {

            Console.CursorVisible = true;
            Console.WriteLine("Ready...");

            var col = Console.ForegroundColor;

            // Now drop to a command prompt
            while (!m_exitRequested)
            {
                this.Prompt();
                var cmd = Console.ReadLine();

                Console.ForegroundColor = this.GetResponseColor();
                if (String.IsNullOrEmpty(cmd)) continue;

                // Get tokens / parms
                var tokens = cmd.Split(' ').ToArray();
                List<String> tToken = new List<string>() { tokens[0] };
                String sstr = String.Empty;
                foreach (var tkn in tokens.Skip(1))
                {
                    if (tkn.StartsWith("'") && tkn.EndsWith("'"))
                        tToken.Add(tkn.Substring(1, tkn.Length - 2));
                    else if (tkn.StartsWith("'"))
                        sstr = tkn.Substring(1);
                    else if (sstr != String.Empty && tkn.EndsWith("'"))
                    {
                        sstr += " " + tkn.Substring(0, tkn.Length - 1);
                        tToken.Add(sstr);
                        sstr = String.Empty;
                    }
                    else if (sstr != String.Empty)
                        sstr += " " + tkn;
                    else
                        tToken.Add(tkn);
                }
                tokens = tToken.ToArray();

                // Get tokens
                MethodInfo cmdMi = null;
                if (!this.m_commandlets.TryGetValue(tokens[0], out cmdMi))
                    Console.Error.WriteLine("ERR: Command {0} with {1} parms not found", tokens[0], tokens.Length - 1);
                else
                {
                    var parmValues = tokens.Length > 1 ? tokens.OfType<Object>().Skip(1).ToArray() : null;

                    try
                    {
                        cmdMi.Invoke(this, parmValues);
                    }
                    catch (Exception e)
                    {
                        this.PrintStack(e);
                    }
                }

                Console.ForegroundColor = col;
            }
        }

        /// <summary>
        /// Show current version of console
        /// </summary>
        [AdminCommand("ver", "Shows current Admin Console Version")]
        public void Version()
        {
            Console.WriteLine("Open Immunize Adminisration & Security Console v{0}", typeof(Program).Assembly.GetName().Version);
            Console.WriteLine("Copyright (C) 2015 - 2017, Mohawk College of Applied Arts and Technology");
        }

        /// <summary>
        /// Clear the console
        /// </summary>
        [AdminCommand("clear", "Clears the screen")]
        public void Clear()
        {
            Console.Clear();
        }

        /// <summary>
        /// Get help
        /// </summary>
        [AdminCommand("help", "Shows help")]
        public void Help()
        {

            foreach (var mi in this.m_commandlets.OrderBy(o => o.Key))
            {
                var itm = mi.Value.GetCustomAttribute<AdminCommandAttribute>();
                if (itm == null || String.IsNullOrEmpty(itm.Description)) continue;
                Console.Write("{0:2} {1}", itm.Command, String.Join(" ", mi.Value.GetParameters().Select(o => $"[{o.Name}]")));
                Console.WriteLine("{0}{1}", new String(' ', 50 - Console.CursorLeft), itm.Description);

            }
        }

        /// <summary>
        /// Identifies current authentication principal
        /// </summary>
        [AdminCommand("whoami", "Identifies the current authentication principal")]
        public void Whoami()
        {
            Console.WriteLine(AuthenticationContext.Current.Principal.Identity.Name);
        }

        /// <summary>
        /// Exit the debugger
        /// </summary>
        [AdminCommand("exit", "Quits the administrative shell")]
        public virtual void Exit()
        {
            Console.ResetColor();
            Console.Clear();
            this.m_exitRequested = true;
            Environment.Exit(0);
        }


    }
}
