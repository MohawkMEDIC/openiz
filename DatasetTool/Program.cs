/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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


            Console.WriteLine("{0} {1} {2}", DateTime.Now, DateTimeOffset.Now, (DateTimeOffset)DateTime.Now);
            var consoleParms = new ParameterParser<ConsoleParameters>().Parse(args);

            var tool = typeof(Program).Assembly.ExportedTypes.FirstOrDefault(o => o.Name == consoleParms.ToolName);
            if (tool == null)
                Console.WriteLine("Could not find tool: {0}", consoleParms.ToolName);
            else
            {
                var operation = tool.GetMethod(consoleParms.OperationName);
                if (operation == null)
                    Console.WriteLine("Tool {0} does not have operation named {1}", consoleParms.ToolName, consoleParms.OperationName);
                else
                    operation.Invoke(null, new Object[] { args });
            }

            Console.ReadKey();
        }
    }
}
