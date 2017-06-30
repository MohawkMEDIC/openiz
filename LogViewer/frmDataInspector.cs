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
using LogViewer.Inspectors;
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
    public partial class frmDataInspector : Form
    {

        private String m_data = null;

        public frmDataInspector(String data)
        {
            InitializeComponent();
            this.m_data = data;
            foreach (var dataInspector in typeof(frmDataInspector).Assembly.GetTypes().Where(o => !o.IsAbstract && typeof(DataInspectorBase).IsAssignableFrom(o)))
                this.cbxViewer.Items.Add(Activator.CreateInstance(dataInspector));
            this.txtDecode.Text = new TextDataInspector().Inspect(data);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbxViewer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtDecode.Text = (cbxViewer.SelectedItem as DataInspectorBase).Inspect(this.m_data);
            }
            catch { }
        }
    }
}
