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
using MohawkCollege.Util.Console.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OizDevTool
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// The The name of the tool
        /// </summary>
        [Parameter("tool")]
        [Description("Identifies the data tool which should be executed")]
        public String ToolName { get; set; }

        /// <summary>
        /// Operation name
        /// </summary>
        [Parameter("operation")]
        [Description("Identifies the specified data tool operation to be executed")]
        public String OperationName { get; set; }

        /// <summary>
        /// Show help and exit
        /// </summary>
        [Parameter("help")]
        [Parameter("?")]
        [Description("Shows this help and exits")]
        public bool Help { get; set; }

        /// <summary>
        /// Override config
        /// </summary>
        [Description("Instructs the tool to use oizdt.exe.config rather than openiz.config")]
        [Parameter("c")]
        [Parameter("selfconfig")]
        public bool CustomConfig { get; set; }
    }
}
