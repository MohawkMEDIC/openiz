using MARC.HI.EHRS.SVC.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ
{
    /// <summary>
    /// Open immunize service
    /// </summary>
    public partial class OpenIZ : ServiceBase
    {

        public OpenIZ()
        {
            // Service Name
            this.ServiceName = "Client Registry";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ExitCode = ServiceUtil.Start(typeof(Program).GUID);
            if (ExitCode != 0)
                Stop();
        }

        protected override void OnStop()
        {
            ServiceUtil.Stop();
        }
    }
}
