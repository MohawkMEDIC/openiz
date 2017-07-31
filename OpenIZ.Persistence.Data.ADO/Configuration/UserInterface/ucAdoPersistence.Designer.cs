namespace OpenIZ.Persistence.Data.ADO.Configuration.UserInterface
{
    partial class ucAdoPersistence
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MARC.HI.EHRS.SVC.Configuration.Data.DbConnectionString dbConnectionString1 = new MARC.HI.EHRS.SVC.Configuration.Data.DbConnectionString();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbxAutoInsert = new System.Windows.Forms.CheckBox();
            this.cbxAutoUpdate = new System.Windows.Forms.CheckBox();
            this.cbxTrace = new System.Windows.Forms.CheckBox();
            this.dbSelector = new MARC.HI.EHRS.SVC.Configuration.UI.DatabaseSelector();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(411, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database Connection";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(0, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(411, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Extended Options";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbxAutoInsert);
            this.panel1.Controls.Add(this.cbxAutoUpdate);
            this.panel1.Controls.Add(this.cbxTrace);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 179);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 84);
            this.panel1.TabIndex = 6;
            // 
            // cbxAutoInsert
            // 
            this.cbxAutoInsert.AutoSize = true;
            this.cbxAutoInsert.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbxAutoInsert.Location = new System.Drawing.Point(0, 54);
            this.cbxAutoInsert.Name = "cbxAutoInsert";
            this.cbxAutoInsert.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.cbxAutoInsert.Size = new System.Drawing.Size(411, 27);
            this.cbxAutoInsert.TabIndex = 2;
            this.cbxAutoInsert.Text = "Automatically insert dependent objects from clients";
            this.cbxAutoInsert.UseVisualStyleBackColor = true;
            // 
            // cbxAutoUpdate
            // 
            this.cbxAutoUpdate.AutoSize = true;
            this.cbxAutoUpdate.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbxAutoUpdate.Location = new System.Drawing.Point(0, 27);
            this.cbxAutoUpdate.Name = "cbxAutoUpdate";
            this.cbxAutoUpdate.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.cbxAutoUpdate.Size = new System.Drawing.Size(411, 27);
            this.cbxAutoUpdate.TabIndex = 1;
            this.cbxAutoUpdate.Text = "Automatically update on insert (when keys match)";
            this.cbxAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // cbxTrace
            // 
            this.cbxTrace.AutoSize = true;
            this.cbxTrace.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbxTrace.Location = new System.Drawing.Point(0, 0);
            this.cbxTrace.Name = "cbxTrace";
            this.cbxTrace.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.cbxTrace.Size = new System.Drawing.Size(411, 27);
            this.cbxTrace.TabIndex = 0;
            this.cbxTrace.Text = "Trace SQL statements to log file";
            this.cbxTrace.UseVisualStyleBackColor = true;
            // 
            // dbSelector
            // 
            dbConnectionString1.Database = "";
            dbConnectionString1.Host = "";
            dbConnectionString1.Name = null;
            dbConnectionString1.Password = "";
            dbConnectionString1.Provider = null;
            dbConnectionString1.UserName = "";
            this.dbSelector.ConnectionString = dbConnectionString1;
            this.dbSelector.Dock = System.Windows.Forms.DockStyle.Top;
            this.dbSelector.Location = new System.Drawing.Point(0, 21);
            this.dbSelector.MinimumSize = new System.Drawing.Size(0, 137);
            this.dbSelector.Name = "dbSelector";
            this.dbSelector.Size = new System.Drawing.Size(411, 137);
            this.dbSelector.TabIndex = 8;
            // 
            // ucAdoPersistence
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dbSelector);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(339, 292);
            this.Name = "ucAdoPersistence";
            this.Size = new System.Drawing.Size(411, 292);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbxAutoUpdate;
        private System.Windows.Forms.CheckBox cbxTrace;
        private System.Windows.Forms.CheckBox cbxAutoInsert;
        private MARC.HI.EHRS.SVC.Configuration.UI.DatabaseSelector dbSelector;
    }
}
