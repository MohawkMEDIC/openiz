using MARC.HI.EHRS.SVC.Core;
using MARC.HI.EHRS.SVC.Core.Services;
using OpenIZ.BusinessRules.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Core.Services.Daemons
{
    /// <summary>
    /// A daemon which loads business rules from the applet manager
    /// </summary>
    public class AppletBusinessRulesDaemon : IDaemonService
    {

        /// <summary>
        /// Indicates whether the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        /// <summary>
        /// Start the service which will register items with the business rules handler
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);
            ApplicationContext.Current.Started += (o, e) =>
            {
                ApplicationServiceContext.Current = ApplicationContext.Current;
                if (ApplicationContext.Current.GetService<IDataReferenceResolver>() == null)
                    ApplicationContext.Current.AddServiceProvider(typeof(AppletDataReferenceResolver));
                new AppletBusinessRuleLoader().LoadRules();
            };
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stopping
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
