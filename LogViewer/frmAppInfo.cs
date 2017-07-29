using OpenIZ.Core.Model.AMI.Diagnostics;
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
using System.Xml.Serialization;

namespace LogViewer
{
    public partial class frmAppInfo : Form
    {

        // Diagnostic report
        private DiagnosticApplicationInfo m_report;

        public frmAppInfo(String fileName)
        {
            InitializeComponent();

            try
            {
                using (var str = File.OpenRead(fileName))
                {
                    this.m_report = new XmlSerializer(typeof(DiagnosticApplicationInfo)).Deserialize(str) as DiagnosticApplicationInfo;
                    this.PopulateData();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred loading the Diagnostic Report: {0}", e.Message);
            }
        }

        /// <summary>
        /// Populate data
        /// </summary>
        private void PopulateData()
        {
            lbl64Bit.DataBindings.Add(nameof(Label.Text), this.m_report.EnvironmentInfo, nameof(DiagnosticEnvironmentInfo.Is64Bit));
            lblNetVersion.DataBindings.Add(nameof(Label.Text), this.m_report.EnvironmentInfo, nameof(DiagnosticEnvironmentInfo.Version));
            lblOperatingSystem.DataBindings.Add(nameof(Label.Text), this.m_report.EnvironmentInfo, nameof(DiagnosticEnvironmentInfo.OSVersion));
            lblUsedMemory.DataBindings.Add(nameof(Label.Text), this.m_report.EnvironmentInfo, nameof(DiagnosticEnvironmentInfo.UsedMemory));
            lblProcessorCount.DataBindings.Add(nameof(Label.Text), this.m_report.EnvironmentInfo, nameof(DiagnosticEnvironmentInfo.ProcessorCount));

            lblOpenIZInfoVersion.DataBindings.Add(nameof(Label.Text), this.m_report, nameof(DiagnosticApplicationInfo.InformationalVersion));
            lblOpenIZVersion.DataBindings.Add(nameof(Label.Text), this.m_report, nameof(DiagnosticApplicationInfo.Version));
            lblSku.DataBindings.Add(nameof(Label.Text), this.m_report, nameof(DiagnosticApplicationInfo.Product));

            foreach(var asm in this.m_report.Assemblies)
            {
                var lvi = lvAsm.Items.Add(asm.Name);
                lvi.SubItems.Add(asm.Version);
                lvi.SubItems.Add(asm.Info);
            }

            foreach (var fil in this.m_report.FileInfo)
            {
                var lvi = lvFiles.Items.Add(fil.FileDescription);
                lvi.SubItems.Add(fil.FileSize.ToString("#,###,##0"));
            }

            foreach(var sync in this.m_report.SyncInfo)
            {
                var lvi = lvSync.Items.Add(sync.ResourceName);
                lvi.SubItems.Add(sync.LastSync.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                lvi.SubItems.Add(sync.Etag);
                lvi.SubItems.Add(sync.Filter);
            }
        }

        /// <summary>
        /// Close the view
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
