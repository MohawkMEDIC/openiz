using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Attributes;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Wcf;
using Swaggerator.Attributes;
using SwaggerWcf;
using SwaggerWcf.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Messaging.Discovery.Swagger
{
    /// <summary>
    /// Represents a daemon service that exports Swagger documentation
    /// </summary>
    [Description("Swagger API Documentation")]
    [TraceSource("OpenIZ.Messaging.Discovery.Swagger")]
    public class SwaggerDaemonService : IDaemonService
    {
        // IMSI Trace host
        private TraceSource m_traceSource = new TraceSource("OpenIZ.Messaging.Discovery.Swagger");

        // web host
        private WebServiceHost m_webHost;

        /// <summary>
        /// Returns true if the service is running
        /// </summary>
        public bool IsRunning { get { return this.m_webHost != null; } }

        /// <summary>
        /// Fired when service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when service has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when service is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Start the service host
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            ApplicationContext.Current.Started += (o, e) =>
             {

                 this.m_traceSource.TraceInformation("Starting Swagger API documentation daemon...");
                 var apis = ApplicationContext.Current.GetServices().OfType<IApiEndpointProvider>();

                 var info = new Info
                 {
                     Description = "OpenIZ REST Endpoints Documentation",
                     Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                     Title = $"Open Immunize ({Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion})"
                     // etc
                 };


                 var wcUrl = ApplicationContext.Current.GetService<IConfigurationManager>().AppSettings["wildcardUrl"];
                 var authUrl = apis.FirstOrDefault(a => a.ApiType == ServiceEndpointType.AuthenticationService)?.Url.FirstOrDefault();

                 var security = new SecurityDefinitions
                    {
                     {
                        "OpenIZ Auth", new SecurityAuthorization
                        {
                          Type = "oauth2",
                          Name = "auth",
                          Description = "Forces authentication with credentials via an oauth gateway",
                          Flow = "password",
                          TokenUrl = authUrl.Replace("0.0.0.0", wcUrl) + "/oauth2_token",
                          Scopes = apis.Where(a=>a.Capabilities.HasFlag(ServiceEndpointCapabilities.BearerAuth) && a.Url.Any()).ToDictionary(a=>a.Url.FirstOrDefault().Replace("0.0.0.0", wcUrl), a=>a.ApiType.ToString()),
                          AuthorizationUrl = authUrl.Replace("0.0.0.0", wcUrl) + "/oauth2_token"
                        }
                      }
                    };

                 SwaggerWcfEndpoint.Configure(info, security);
                 this.m_webHost = new WebServiceHost(typeof(SwaggerWcfEndpoint));
                 this.m_traceSource.TraceInformation("\tStarting Swagger API documentation {0}...", this.m_webHost.Description.Endpoints.First().Address);
                 this.m_webHost.Open();
                 this.Started?.Invoke(this, EventArgs.Empty);
             };

            return true;
        }

        /// <summary>
        /// Stop the service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            if (this.IsRunning)
            {
                this.m_traceSource.TraceInformation("Stopping Swagger...");
                this.m_webHost.Close();
                this.m_webHost = null;
            }
            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
