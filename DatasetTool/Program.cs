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
 * Date: 2016-8-2
 */
using MARC.HI.EHRS.SVC.Core;
using MohawkCollege.Util.Console.Parameters;
using OpenIZ.Core.Services.Impl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OizDevTool
{
    class Program
    {

        static void Main(string[] args)
        {

            AppDomain.CurrentDomain.SetData(
               "DataDirectory",
               Path.GetDirectoryName(typeof(Program).Assembly.Location));


            Console.WriteLine("Open Immunize Development Tool v{0}", typeof(Program).Assembly.GetName().Version);
            Console.WriteLine("Copyright (C) 2015 - 2017, Mohawk College of Applied Arts and Technology");

            try
            {
                var consoleParms = new ParameterParser<ConsoleParameters>().Parse(args);

                if (!consoleParms.CustomConfig)
                {
                    Console.WriteLine("Using openiz config");
                    ApplicationContext.Current.AddServiceProvider(typeof(FileConfigurationService));
                }
                else
                {
                    Console.WriteLine("Using oizdt config");
                }
                if (consoleParms.Help)
                    PrintHelp(consoleParms.ToolName);
                else
                {
                    var tool = typeof(Program).Assembly.ExportedTypes.FirstOrDefault(o => o.Name == consoleParms.ToolName);
                    if (tool == null)
                        Console.WriteLine("Could not find tool: {0}", consoleParms.ToolName);
                    else
                    {
                        var operation = tool.GetMethod(consoleParms.OperationName);
                        if (operation == null)
                            Console.WriteLine("Tool {0} does not have operation named {1}", consoleParms.ToolName, consoleParms.OperationName);
                        else
                        {
                            bool exitDoodad = false;
                            try
                            {
                                Console.CursorVisible = false;
                                // Draws a doo-dad on the screen
                                Task<Action> doodadTask = new Task<Action>(() =>
                                {
                                    int cp = 0;
                                    char[] b = { '|', '/', '-', '\\' };
                                    int lastCursorTop = 0;
                                    while (!exitDoodad)
                                    {
                                        if (Console.CursorTop == lastCursorTop)
                                        {
                                            Console.CursorLeft = 0;
                                            Console.Write(b[cp++ % b.Length]);
                                            Console.CursorLeft = 0;
                                            Thread.Sleep(100);
                                        }
                                        else
                                        {
                                            lastCursorTop = Console.CursorTop;
                                            Thread.Sleep(2000);
                                        }
                                    }
                                    return null;
                                });
                                if(operation.GetCustomAttribute<InteractiveAttribute>() == null)
                                    doodadTask.Start();
                                operation.Invoke(null, new Object[] { args });

                            }
                            finally
                            {
                                Console.CursorVisible = true;

                                exitDoodad = true;
                            }
                        }
                    }
                }

                if (System.Diagnostics.Debugger.IsAttached)
                {
                    Console.WriteLine("Program complete, press any key to exit...");
                    Console.ReadKey();
                }
            }
            catch(Exception e)
            {
#if DEBUG
                Console.Error.WriteLine(e);
#else
                var ex = e;
                while (ex != null)
                {
                    Console.Error.WriteLine("{0} @ STACK: {1}", ex.Message, ex.StackTrace);
                    ex = ex.InnerException;
                }
#endif
            }
        }

        /// <summary>
        /// Print help
        /// </summary>
        static void PrintHelp(String toolName)
        {

            if (string.IsNullOrEmpty(toolName)) {
                Console.WriteLine("\r\nCore Options:");
                new ParameterParser<ConsoleParameters>().WriteHelp(Console.Out);

                // Output help for each tool
                Console.WriteLine("\r\n\r\nInstalled --tool Options:\r\n");
                foreach (var t in typeof(Program).Assembly.GetTypes().Where(o=>o.GetCustomAttribute<DescriptionAttribute>() != null && o.IsAbstract && o.IsSealed))
                {
                    Console.WriteLine("{0}{1}{2}", t.Name, new String(' ', 20 - t.Name.Length) ,t.GetCustomAttribute<DescriptionAttribute>().Description);
                }

                Console.WriteLine("\r\nFor more help on a specified tool use: --help --tool=[toolname]");
            }
            else
            {
                var type = typeof(Program).Assembly.GetTypes().FirstOrDefault(o => o.Name == toolName);
                if (type == null)
                {
                    Console.WriteLine("Error: Tool {0} is invalid", toolName);
                    return;
                }

                // Output tool
                Console.WriteLine("\r\n{1}\r\nUsage : --tool={0} --operation=[operationName] {{options}}", type.Name, type.GetCustomAttribute<DescriptionAttribute>().Description);

                // Output operations
                Console.WriteLine("\r\n");
                foreach(var mi in type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(o=>o.GetCustomAttribute<DescriptionAttribute>() != null))
                {
                    Console.WriteLine("Operation: {0}", mi.Name);
                    Console.WriteLine(mi.GetCustomAttribute<DescriptionAttribute>().Description);
                    Console.WriteLine("Usage: --tool={0} --operation={1} {{options}}", type.Name, mi.Name);

                    if (mi.GetCustomAttribute<ParameterClassAttribute>() != null)
                    {
                        Console.WriteLine("\r\nWhere {options} are:");

                        dynamic clz =Activator.CreateInstance(typeof(ParameterParser<>).MakeGenericType(mi.GetCustomAttribute<ParameterClassAttribute>().ParameterClass));
                        clz.GetType().GetMethod("WriteHelp").Invoke(clz, new object[] { Console.Out });

                    }

                    Console.WriteLine();

                    foreach(var ex in mi.GetCustomAttributes<ExampleAttribute>())
                    {
                        Console.WriteLine("Example: {0} \r\n{1} --tool={2} --operation={3} {4}\r\n", ex.Description, Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location), type.Name, mi.Name, ex.ExampleText);
                    }

                    Console.WriteLine("----\r\n");
                }
            }

        }
    }
}
