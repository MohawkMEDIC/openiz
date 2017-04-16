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
                        OpenIZ.BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.AddRules(sr);
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
