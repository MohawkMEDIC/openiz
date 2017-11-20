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
using MARC.HI.EHRS.SVC.Configuration.UI;
using System.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace OpenIZ.Core.Configuration.UI
{
    public partial class ucCoreSettings : UserControl
    {

        // Configuration
        private OpenIzConfiguration m_configuration = null;

        // Keys for display
        private ObservableCollection<ListItem<SecurityKey, String>> m_keys = new ObservableCollection<ListItem<SecurityKey, string>>();

        // Publisher binding
        private BindingList<String> m_publisherBinding;

        // Issuers binding
        private BindingList<ListItem<SecurityKey, String>> m_issuersBinding;

        /// <summary>
        /// Gets or sets the configuration
        /// </summary>
        public OpenIzConfiguration Configuration {
            get
            {
                if(this.cbxSecurity.SelectedIndex == 0)
                    this.m_configuration.Security.ClaimsAuth.IssuerKeys = this.m_keys.ToDictionary(o => o.Display, o => o.Key);
                return this.m_configuration;
            }
            set
            {
                this.m_configuration = value;
                this.numThreadPool.DataBindings.Clear();
                this.numThreadPool.DataBindings.Add(nameof(NumericUpDown.Value), this.m_configuration, nameof(OpenIzConfiguration.ThreadPoolSize));
                this.chkUnsignedApplets.DataBindings.Clear();
                this.chkUnsignedApplets.DataBindings.Add(nameof(CheckBox.Checked), this.m_configuration.Security, nameof(OpenIzSecurityConfiguration.AllowUnsignedApplets));
                this.lsbPublishers.DataBindings.Clear();
                this.m_publisherBinding = new BindingList<String>(this.m_configuration.Security.TrustedPublishers);
                this.lsbPublishers.DataSource = this.m_publisherBinding;
                this.txtRealm.DataBindings.Clear();
                this.txtRealm.DataBindings.Add(nameof(TextBox.Text), (object)this.m_configuration.Security.BasicAuth ?? this.m_configuration.Security.ClaimsAuth, "Realm");
                this.cbxSecurity.SelectedIndex = this.m_configuration.Security.BasicAuth != null ? 1 : 0;

                if (this.m_configuration.Security.ClaimsAuth != null)
                {
                    this.m_keys = new ObservableCollection<ListItem<SecurityKey, string>>(this.m_configuration.Security.ClaimsAuth.IssuerKeys.Select(o => new ListItem<SecurityKey, String>(o.Value, o.Key)));
                    this.m_issuersBinding = new BindingList<ListItem<SecurityKey, String>>(m_keys);
                    this.lsbPublishers.DataSource = this.m_publisherBinding;
                    this.lsbOAuth.DataSource = this.m_issuersBinding;
                }
            }
        }

        /// <summary>
        /// Constructor for the object
        /// </summary>
        public ucCoreSettings()
        {
            InitializeComponent();

            this.numThreadPool.Minimum = Environment.ProcessorCount;
            this.numThreadPool.Maximum = Environment.ProcessorCount * 8;
            this.numThreadPool.Increment = Environment.ProcessorCount;

            cbxHashing.Items.Clear();
            var hashers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(o => o.ExportedTypes).Where(o => typeof(IPasswordHashingService).IsAssignableFrom(o) && !o.IsInterface).Select(o=>new ListItem<Type, String>(o, o.Name));
            cbxHashing.Items.AddRange(hashers.ToArray());
        }

        // Add publisher
        private void btnAddPublisher_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtPublisher.Text))
                this.m_publisherBinding.Add(txtPublisher.Text);
        }

        private void btnAddOauth_Click(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(txtIssuer.Text) &&
                !String.IsNullOrEmpty(txtSymmSecret.Text))
            {
                this.m_issuersBinding.Add(new ListItem<SecurityKey, string>(new InMemorySymmetricSecurityKey(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(txtSymmSecret.Text))), txtIssuer.Text));
            }
        }
    }

}
