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
using OizDevTool.Debugger;
using OpenIZ.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool
{
    /// <summary>
    /// Represents a base class for interactive shells
    /// </summary>
    public abstract class InteractiveBase
    {
        // Exit debugger
        private bool m_exitRequested = false;

        protected object m_scopeObject = null;
        protected string m_prompt = "dbg >";
        private ConsoleColor m_promptColor = Console.ForegroundColor;

        /// <summary>
        /// Sets the root path
        /// </summary>
        public InteractiveBase()
        {
            AuthenticationContext.Current = new AuthenticationContext(AuthenticationContext.AnonymousPrincipal);
        }

        /// <summary>
        /// Get response color
        /// </summary>
        protected ConsoleColor GetResponseColor()
        {
            if(Console.BackgroundColor == ConsoleColor.Black)
                return Console.ForegroundColor != ConsoleColor.Cyan ? ConsoleColor.Cyan : ConsoleColor.Magenta;
            else
                return Console.ForegroundColor != ConsoleColor.Blue ? ConsoleColor.Blue : ConsoleColor.Red;
        }

        /// <summary>
        /// Prompt
        /// </summary>
        protected void Prompt()
        {
            Console.ResetColor();
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
                var cmdMi = this.GetType().GetMethods().Where(o => o.GetCustomAttribute<CommandAttribute>()?.Command == tokens[0] && o.GetParameters().Length == tokens.Length - 1).FirstOrDefault();
                if (cmdMi == null)
                    Console.Error.WriteLine("ERR: Command {0} with {1} parms not found", tokens[0], tokens.Length - 1);
                else
                {
                    var parmValues = tokens.Length > 1 ? tokens.OfType<Object>().Skip(1).ToArray() : null;

                    try
                    {
                        if (cmdMi.ReturnType == typeof(void))
                            cmdMi.Invoke(this, parmValues);
                        else
                            this.m_scopeObject = cmdMi.Invoke(this, parmValues);
                    }
                    catch (Exception e)
                    {
                        this.PrintStack(e);
                    }
                }

                Console.ForegroundColor = col;
            }
        }

        [Command("say", "")]
        public void BeaverSay(String phrase)
        {
            String[] beaver = { "       .-\"\"\"-.__   ", "      /      ' o'\\", "   ,-;  '.  :   _c", "  :_.\"\\._ ) ::-", "         \"\"m \"m" };
            String[] bubble = { $"\t/{new String('-', phrase.Length + 4)}\\", $"\t|  {phrase}  |", $"\t\\{new String('-', phrase.Length + 4)}/" };


            foreach (var itm in bubble)
                Console.WriteLine(itm);
            foreach (var itm in beaver)
                Console.WriteLine(itm);

        }

        /// <summary>
        /// Get help
        /// </summary>
        [Command("?", "Shows help and exits")]
        public void Help()
        {

            foreach (var mi in this.GetType().GetMethods().OrderBy(o => o.Name))
            {
                var itm = mi.GetCustomAttribute<CommandAttribute>();
                if (itm == null || String.IsNullOrEmpty(itm.Description)) continue;
                Console.Write("{0:2} {1}", itm.Command, String.Join(" ", mi.GetParameters().Select(o => $"[{o.Name}]")));
                Console.WriteLine("{0}{1}", new String(' ', 50 - Console.CursorLeft), itm.Description);

            }
        }

        /// <summary>
        /// Identifies current authentication principal
        /// </summary>
        [Command("whoami", "Identifies the current authentication principal")]
        public void Whoami()
        {
            Console.WriteLine(AuthenticationContext.Current.Principal.Identity.Name);
        }
        
        /// <summary>
        /// Exit the debugger
        /// </summary>
        [Command("q", "Quits the shell")]
        public virtual void Exit()
        {
            this.m_exitRequested = true;
            //Environment.Exit(0);
        }


    }
}
