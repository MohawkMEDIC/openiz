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
 * Date: 2017-4-14
 */
using OpenIZ.Core;
using OpenIZ.Core.Applets.Services;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Json.Formatter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.BusinessRules.JavaScript
{
    /// <summary>
    /// Represents a utility class which loads business rules from the applet manager
    /// </summary>
    public class AppletBusinessRuleLoader
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(AppletBusinessRuleLoader));

        /// <summary>
        /// Load all rules from applets
        /// </summary>
        public void LoadRules()
        {
            try
            {
                var appletManager = ApplicationServiceContext.Current.GetService(typeof(IAppletManagerService)) as IAppletManagerService;

                foreach (var itm in appletManager.Applets.SelectMany(a => a.Assets).Where(a => a.Name.StartsWith("rules/")))
                    using (StreamReader sr = new StreamReader(new MemoryStream(appletManager.Applets.RenderAssetContent(itm))))
                    {
                        OpenIZ.BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.AddRules(itm.Name, sr);
                        this.m_tracer.TraceInfo("Added rules from {0}", itm.Name);
                    }
                OpenIZ.BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.Bridge.Serializer.LoadSerializerAssembly(typeof(ActExtensionViewModelSerializer).GetTypeInfo().Assembly);

            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error on startup: {0}", ex);
                throw new InvalidOperationException("Could not start business rules engine manager service", ex);
            }
        }
    }
}
