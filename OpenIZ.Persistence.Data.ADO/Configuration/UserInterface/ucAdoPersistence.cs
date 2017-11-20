using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MARC.HI.EHRS.SVC.Configuration.Data;

namespace OpenIZ.Persistence.Data.ADO.Configuration.UserInterface
{
    public partial class ucAdoPersistence : UserControl
    {

        private AdoConfiguration m_configurationObject = null;

        /// <summary>
        /// Get or set the connection string
        /// </summary>
        public DbConnectionString ConnectionString
        {
            get
            {
                return this.dbSelector.ConnectionString;
            }
            set {
                this.dbSelector.ConnectionString = value;
            }
        }
        
        /// <summary>
        /// Gets the configuration object
        /// </summary>
        public AdoConfiguration ConfigurationObject
        {
            get
            {
                return this.m_configurationObject;
            }
            set
            {

                this.cbxAutoInsert.DataBindings.Clear();
                this.cbxAutoUpdate.DataBindings.Clear();
                this.cbxTrace.DataBindings.Clear();
                this.m_configurationObject = value;
                this.cbxAutoInsert.DataBindings.Add(nameof(CheckBox.Checked), this.ConfigurationObject, nameof(AdoConfiguration.AutoInsertChildren));
                this.cbxAutoUpdate.DataBindings.Add(nameof(CheckBox.Checked), this.ConfigurationObject, nameof(AdoConfiguration.AutoUpdateExisting));
                this.cbxTrace.DataBindings.Add(nameof(CheckBox.Checked), this.ConfigurationObject, nameof(AdoConfiguration.TraceSql));
            }
        }

        /// <summary>
        /// Creates a new persistence panel
        /// </summary>
        public ucAdoPersistence()
        {
            InitializeComponent();

            this.dbSelector.Validated += (o, e) =>
            {
                this.m_configurationObject.ReadWriteConnectionString = this.m_configurationObject.ReadonlyConnectionString = this.dbSelector.ConnectionString.Name;
            };
        }

    }
}
