using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogViewer
{
    public partial class frmOpenJira : Form
    {

        /// <summary>
        /// Issue identifier
        /// </summary>
        public String IssueId
        {
            get
            {
                return lsvIssues.SelectedItems[0].Text;
            }
        }

        public frmOpenJira()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cancel the request
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
