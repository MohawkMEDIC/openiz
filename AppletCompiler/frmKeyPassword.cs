using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppletCompiler
{
    public partial class frmKeyPassword : Form
    {
        public frmKeyPassword(String keyFile)
        {
            InitializeComponent();
            label1.Text = $"Enter private key password for {Path.GetFileNameWithoutExtension(keyFile)}";
        }

        /// <summary>
        /// Password text
        /// </summary>
        public string Password { get { return txtPassword.Text; } }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
