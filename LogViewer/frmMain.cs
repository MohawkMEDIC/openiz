/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-3-31
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogViewer
{
    
    public partial class frmMain : Form
    {

        private List<LogEvent> m_logEvent = null;
        
        public frmMain()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "All Log Files|*.log;*.log.*;*.log*.gz|Log Files (*.log)|*.log|Historical Log Files (*.log.*)|*.log.*|Compressed Log Files (*.log.gz)|*.log*.gz",
                Title = "Open Log File"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(ofd.FileName) == ".gz")
                    this.m_logEvent = LogEvent.LoadGz(ofd.FileName);
                else
                    this.m_logEvent = LogEvent.Load(ofd.FileName);
                this.RefreshFile();
            }
        }

        private void DrawList(Func<LogEvent, bool> items)
        {
            this.lsvEvents.Items.Clear();
            this.lsvEvents.Items.AddRange(
                this.m_logEvent
                .Where(items)
                .Select(o => new ListViewItem(new String[] {
                    o.Sequence.ToString(),
                    o.Level.ToString(),
                    o.Source,
                    o.Date.ToString("yyyy-MMM-dd HH:mm:ss.ffff"),
                    o.Thread,
                    o.Message
                })).ToArray()
            );
        }

        private void Filter()
        {
            this.DrawList(o => (trvSource.SelectedNode != null && o.Source.StartsWith(trvSource.SelectedNode.Name)|| trvSource.SelectedNode == null)
                && (!String.IsNullOrEmpty(cbxLevel.SelectedItem?.ToString()) && o.Level.ToString() == cbxLevel.SelectedItem.ToString() || String.IsNullOrEmpty(cbxLevel.SelectedItem?.ToString()))
                && (!String.IsNullOrEmpty(cbxThread.SelectedItem?.ToString()) && o.Thread == cbxThread.SelectedItem.ToString() || String.IsNullOrEmpty(cbxThread.SelectedItem?.ToString()))
                && (!String.IsNullOrEmpty(txtSearch.Text) && o.Message.ToLower().Contains(txtSearch.Text.ToLower()) || String.IsNullOrEmpty(txtSearch.Text)));
        }

        /// <summary>
        /// Refresh file data
        /// </summary>
        private void RefreshFile()
        {
            this.trvSource.Nodes.Clear();
            //this.m_logEvent.Sort((a, b) => a.Source.CompareTo(b.Source));

            cbxLevel.Items.Clear();
            cbxLevel.Items.Add(String.Empty);
            cbxLevel.Items.AddRange(this.m_logEvent.Select(o => o.Level.ToString()).Distinct().ToArray());
            cbxThread.Items.Clear();
            cbxThread.Items.AddRange(this.m_logEvent.Select(o => o.Thread).Distinct().ToArray());

            foreach (var itm in this.m_logEvent)
            {
                // Node info
                String parentName = itm.Source, nodeName = itm.Source;
                if (nodeName.Contains("["))
                    parentName = nodeName = nodeName.Substring(0, itm.Source.IndexOf("["));

                TreeNode node = this.trvSource.Nodes.Find(nodeName, true).FirstOrDefault(),
                    parentNode = this.trvSource.Nodes.Find(parentName, true).FirstOrDefault();

                while (parentNode == null && parentName.Contains(".")) // Go till we find a parent
                {
                    parentName = parentName.Substring(0, parentName.LastIndexOf("."));
                    parentNode = this.trvSource.Nodes.Find(parentName, true).FirstOrDefault();
                }

                // Parent not found
                if (parentNode == null)
                {
                    parentNode = this.trvSource.Nodes.Add(parentName, parentName);
                }

                if (nodeName.Length > parentName.Length)
                {
                    int t = nodeName.IndexOf(".", parentName.Length + 1);
                    while (t != -1)
                    {
                        parentName = nodeName.Substring(0, t);
                        parentNode = parentNode.Nodes.Add(parentName, parentName.Replace(parentNode.Name, ""));
                        t = nodeName.IndexOf(".", t + 1);
                    }
                }

                // Not found?
                if (node == null && nodeName != parentName)
                    node = parentNode.Nodes.Add(nodeName, nodeName.Replace(parentNode.Name, ""));

            }
            this.DrawList(o => o != null);
        }

        
        private void trvSource_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.Filter();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void cbxLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Filter();

        }

        private void cbxThread_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Filter();

        }

        private void lsvEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.lsvEvents.SelectedItems.Count > 0)
            {
                txtText.Text = $"Source: {lsvEvents.SelectedItems[0].SubItems[2].Text}\r\nLevel: {lsvEvents.SelectedItems[0].SubItems[1].Text}\r\n"+
                    $"Date: {lsvEvents.SelectedItems[0].SubItems[3].Text}\r\nThread:{lsvEvents.SelectedItems[0].SubItems[4].Text}\r\nMessage:\r\n\r\n{lsvEvents.SelectedItems[0].SubItems[5].Text}";
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            this.Filter();
        }

        private void showCousinsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsvEvents.SelectedItems.Count > 0)
            {
                var selSequence = Int32.Parse(lsvEvents.SelectedItems[0].SubItems[0].Text);
                this.DrawList(o => Math.Abs(o.Sequence - selSequence) < 20);
            }
        }

        private void showNeighboursOnSameThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsvEvents.SelectedItems.Count > 0)
            {
                var selDate = DateTime.Parse(lsvEvents.SelectedItems[0].SubItems[3].Text);
                var thread = lsvEvents.SelectedItems[0].SubItems[4].Text;
                this.DrawList(o => Math.Abs((o.Date - selDate).TotalMilliseconds) < 1000 && o.Thread == thread);
            }
        }

        private void sameSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsvEvents.SelectedItems.Count > 0)
            {
                var src = lsvEvents.SelectedItems[0].SubItems[2].Text;
                this.DrawList(o => o.Source == src);
            }
        }

        private void sameThreadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsvEvents.SelectedItems.Count > 0)
            {
                var thd = lsvEvents.SelectedItems[0].SubItems[4].Text;

                this.DrawList(o => o.Thread == thd);
            }
        }

        private void sameLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lsvEvents.SelectedItems.Count > 0)
            {
                var el = (EventLevel)Enum.Parse(typeof(EventLevel), lsvEvents.SelectedItems[0].SubItems[1].Text);
                this.DrawList(o => o.Level == el);
            }
        }

        private void decodeBase64ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmDataInspector(txtText.SelectedText ?? txtText.Text).ShowDialog();
        }
    }

}