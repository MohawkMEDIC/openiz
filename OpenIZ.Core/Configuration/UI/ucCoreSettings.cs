using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MARC.HI.EHRS.SVC.Core.Services.Security;

namespace OpenIZ.Core.Configuration.UI
{
    public partial class ucCoreSettings : UserControl
    {
        public ucCoreSettings()
        {
            InitializeComponent();

            this.numThreadPool.Minimum = Environment.ProcessorCount;
            this.numThreadPool.Maximum = Environment.ProcessorCount * 8;
            this.numThreadPool.Increment = Environment.ProcessorCount;

            cbxHashing.Items.Clear();
            var hashers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.ExportedTypes).Where(o => typeof(IPasswordHashingService).IsAssignableFrom(o) && !o.IsInterface).Select(o=>o.Name);
            cbxHashing.Items.AddRange(hashers.ToArray());
        }
    }
}
