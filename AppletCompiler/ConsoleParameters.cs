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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppletCompiler
{
    /// <summary>
    /// Console parameters
    /// </summary>
    public class ConsoleParameters
    {

        /// <summary>
        /// Deploy
        /// </summary>
        [Parameter("deploy")]
        [Description("Deploys the rendered files to the secified directory")]
        public string Deploy { get; set; }
        
        /// <summary>
        /// Language
        /// </summary>
        [Parameter("lang")]
        [Description("The language to render output files in (if rendering)")]
        public string Lang { get; set; }

        /// <summary>
        /// Clean
        /// </summary>
        [Parameter("c")]
        [Description("Instructs the compiler to clean the output directory")]
        public bool Clean { get; set; }

        /// <summary>
        /// Source files
        /// </summary>
        [Parameter("s")]
        [Parameter("source")]
        [Description("Identifies the source files to include in the applet")]
        public String Source { get; set; }

        /// <summary>
        /// Gets or sets the output
        /// </summary>
        [Description("The output applet file")]
        [Parameter("o")]
        [Parameter("output")]
        public String Output { get; set; }


        /// <summary>
        /// Gets or sets the indicator for showing help
        /// </summary>
        [Parameter("?")]
        [Parameter("help")]
        [Description("Shows this help and exits")]
        public bool Help { get; set; }

        /// <summary>
        /// Includes the specified files
        /// </summary>
        [Parameter("i")]
        [Parameter("include")]
        [Description("Includes files from another directory in the applet package")]
        public StringCollection References { get; set; }

        /// <summary>
        /// Optimize the output files
        /// </summary>
        [Parameter("optimize")]
        [Description("When true, optimize (minify) javascript and css")]
        public bool Optimize { get; set; }

        /// <summary>
        /// The key that should be used to sign the applet
        /// </summary>
        [Parameter("keyFile")]
        [Description("The RSA key used to sign the applet")]
        public String SignKey { get; set; }

        /// <summary>
        /// The key used to sign the applet
        /// </summary>
        [Parameter("keyPassword")]
        [Description("The password for the applet signing key")]
        public String SignPassword { get; set; }

        /// <summary>
        /// Compile instruction
        /// </summary>
        [Parameter("compile")]
        [Description("Initiates a compilation of the specified applet source directories")]
        public bool Compile { get; internal set; }

        /// <summary>
        /// Signing instruction
        /// </summary>
        [Parameter("sign")]
        [Description("Signs an already existing applet pak file")]
        public bool Sign { get; internal set; }

        /// <summary>
        /// Embed certificate into the manifest
        /// </summary>
        [Parameter("embedCert")]
        [Description("Embeds the certificate used to sign the package in the applet (recommended for wide-publishing)")]
        public bool EmbedCertificate { get; set; }
    }
}
