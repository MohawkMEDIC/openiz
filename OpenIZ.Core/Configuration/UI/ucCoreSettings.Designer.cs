namespace OpenIZ.Core.Configuration.UI
{
    partial class ucCoreSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.numThreadPool = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.chkUnsignedApplets = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddPublisher = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.txtPublisher = new System.Windows.Forms.TextBox();
            this.lsbPublishers = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbxHashing = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxSecurity = new System.Windows.Forms.ComboBox();
            this.txtRealm = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddOauth = new System.Windows.Forms.Button();
            this.btnDelOauth = new System.Windows.Forms.Button();
            this.lsbOAuth = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtIssuer = new System.Windows.Forms.TextBox();
            this.txtSymmSecret = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreadPool)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
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
            this.label1.Size = new System.Drawing.Size(437, 20);
            this.label1.TabIndex = 26;
            this.label1.Text = "Thread Process Control";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.numThreadPool);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(437, 29);
            this.flowLayoutPanel1.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label2.Size = new System.Drawing.Size(158, 23);
            this.label2.TabIndex = 0;
            this.label2.Text = "Maximum Thread Pool Size:";
            // 
            // numThreadPool
            // 
            this.numThreadPool.Location = new System.Drawing.Point(167, 3);
            this.numThreadPool.Name = "numThreadPool";
            this.numThreadPool.Size = new System.Drawing.Size(120, 20);
            this.numThreadPool.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(0, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(437, 20);
            this.label3.TabIndex = 28;
            this.label3.Text = "Applets";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkUnsignedApplets
            // 
            this.chkUnsignedApplets.AutoSize = true;
            this.chkUnsignedApplets.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkUnsignedApplets.Location = new System.Drawing.Point(0, 69);
            this.chkUnsignedApplets.Name = "chkUnsignedApplets";
            this.chkUnsignedApplets.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.chkUnsignedApplets.Size = new System.Drawing.Size(437, 27);
            this.chkUnsignedApplets.TabIndex = 29;
            this.chkUnsignedApplets.Text = "Allow Unsigned Applets";
            this.chkUnsignedApplets.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 96);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label4.Size = new System.Drawing.Size(129, 23);
            this.label4.TabIndex = 30;
            this.label4.Text = "Registered Publishers";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel1.Controls.Add(this.btnAddPublisher, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnRemove, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPublisher, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lsbPublishers, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 119);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 71F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(437, 100);
            this.tableLayoutPanel1.TabIndex = 31;
            // 
            // btnAddPublisher
            // 
            this.btnAddPublisher.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddPublisher.Location = new System.Drawing.Point(357, 3);
            this.btnAddPublisher.Name = "btnAddPublisher";
            this.btnAddPublisher.Size = new System.Drawing.Size(77, 23);
            this.btnAddPublisher.TabIndex = 0;
            this.btnAddPublisher.Text = "Add";
            this.btnAddPublisher.UseVisualStyleBackColor = true;
            this.btnAddPublisher.Click += new System.EventHandler(this.btnAddPublisher_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnRemove.Location = new System.Drawing.Point(357, 32);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(77, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // txtPublisher
            // 
            this.txtPublisher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPublisher.Location = new System.Drawing.Point(3, 3);
            this.txtPublisher.Name = "txtPublisher";
            this.txtPublisher.Size = new System.Drawing.Size(348, 20);
            this.txtPublisher.TabIndex = 2;
            // 
            // lsbPublishers
            // 
            this.lsbPublishers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbPublishers.FormattingEnabled = true;
            this.lsbPublishers.Location = new System.Drawing.Point(3, 32);
            this.lsbPublishers.Name = "lsbPublishers";
            this.lsbPublishers.Size = new System.Drawing.Size(348, 65);
            this.lsbPublishers.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(0, 219);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(437, 20);
            this.label5.TabIndex = 33;
            this.label5.Text = "Security Mode";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label6.Size = new System.Drawing.Size(114, 27);
            this.label6.TabIndex = 0;
            this.label6.Text = "Security Mode:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.cbxHashing, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbxSecurity, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.txtRealm, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 239);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(437, 84);
            this.tableLayoutPanel2.TabIndex = 35;
            // 
            // cbxHashing
            // 
            this.cbxHashing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbxHashing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHashing.FormattingEnabled = true;
            this.cbxHashing.Items.AddRange(new object[] {
            "Bearer Token Authentication",
            "HTTP Basic Authentication"});
            this.cbxHashing.Location = new System.Drawing.Point(123, 57);
            this.cbxHashing.Name = "cbxHashing";
            this.cbxHashing.Size = new System.Drawing.Size(311, 21);
            this.cbxHashing.TabIndex = 6;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 54);
            this.label11.Name = "label11";
            this.label11.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label11.Size = new System.Drawing.Size(114, 27);
            this.label11.TabIndex = 5;
            this.label11.Text = "Hashing:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(3, 27);
            this.label7.Name = "label7";
            this.label7.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label7.Size = new System.Drawing.Size(114, 27);
            this.label7.TabIndex = 3;
            this.label7.Text = "Realm:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbxSecurity
            // 
            this.cbxSecurity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbxSecurity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSecurity.FormattingEnabled = true;
            this.cbxSecurity.Items.AddRange(new object[] {
            "Bearer Token Authentication",
            "HTTP Basic Authentication"});
            this.cbxSecurity.Location = new System.Drawing.Point(123, 3);
            this.cbxSecurity.Name = "cbxSecurity";
            this.cbxSecurity.Size = new System.Drawing.Size(311, 21);
            this.cbxSecurity.TabIndex = 2;
            // 
            // txtRealm
            // 
            this.txtRealm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRealm.Location = new System.Drawing.Point(123, 30);
            this.txtRealm.Name = "txtRealm";
            this.txtRealm.Size = new System.Drawing.Size(311, 20);
            this.txtRealm.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(0, 323);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(437, 20);
            this.label8.TabIndex = 37;
            this.label8.Text = "Trusted OAUTH 2.0 Issuers";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel3.Controls.Add(this.btnAddOauth, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnDelOauth, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.lsbOAuth, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 343);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.93413F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 67.06586F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(437, 167);
            this.tableLayoutPanel3.TabIndex = 38;
            // 
            // btnAddOauth
            // 
            this.btnAddOauth.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAddOauth.Location = new System.Drawing.Point(357, 29);
            this.btnAddOauth.Name = "btnAddOauth";
            this.btnAddOauth.Size = new System.Drawing.Size(77, 23);
            this.btnAddOauth.TabIndex = 0;
            this.btnAddOauth.Text = "Add";
            this.btnAddOauth.UseVisualStyleBackColor = true;
            this.btnAddOauth.Click += new System.EventHandler(this.btnAddOauth_Click);
            // 
            // btnDelOauth
            // 
            this.btnDelOauth.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDelOauth.Location = new System.Drawing.Point(357, 58);
            this.btnDelOauth.Name = "btnDelOauth";
            this.btnDelOauth.Size = new System.Drawing.Size(77, 23);
            this.btnDelOauth.TabIndex = 1;
            this.btnDelOauth.Text = "Remove";
            this.btnDelOauth.UseVisualStyleBackColor = true;
            // 
            // lsbOAuth
            // 
            this.lsbOAuth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbOAuth.FormattingEnabled = true;
            this.lsbOAuth.Location = new System.Drawing.Point(3, 58);
            this.lsbOAuth.Name = "lsbOAuth";
            this.lsbOAuth.Size = new System.Drawing.Size(348, 106);
            this.lsbOAuth.TabIndex = 3;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.label9, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtIssuer, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.txtSymmSecret, 1, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(348, 49);
            this.tableLayoutPanel4.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(3, 24);
            this.label10.Name = "label10";
            this.label10.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label10.Size = new System.Drawing.Size(114, 25);
            this.label10.TabIndex = 2;
            this.label10.Text = "Symmetric Secret:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(3, 0);
            this.label9.Name = "label9";
            this.label9.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.label9.Size = new System.Drawing.Size(114, 24);
            this.label9.TabIndex = 1;
            this.label9.Text = "Issuer:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtIssuer
            // 
            this.txtIssuer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtIssuer.Location = new System.Drawing.Point(123, 3);
            this.txtIssuer.Name = "txtIssuer";
            this.txtIssuer.Size = new System.Drawing.Size(222, 20);
            this.txtIssuer.TabIndex = 3;
            // 
            // txtSymmSecret
            // 
            this.txtSymmSecret.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSymmSecret.Location = new System.Drawing.Point(123, 27);
            this.txtSymmSecret.Name = "txtSymmSecret";
            this.txtSymmSecret.PasswordChar = '*';
            this.txtSymmSecret.Size = new System.Drawing.Size(222, 20);
            this.txtSymmSecret.TabIndex = 4;
            // 
            // ucCoreSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkUnsignedApplets);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "ucCoreSettings";
            this.Size = new System.Drawing.Size(437, 520);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numThreadPool)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numThreadPool;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkUnsignedApplets;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnAddPublisher;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.TextBox txtPublisher;
        private System.Windows.Forms.ListBox lsbPublishers;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ComboBox cbxHashing;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbxSecurity;
        private System.Windows.Forms.TextBox txtRealm;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnAddOauth;
        private System.Windows.Forms.Button btnDelOauth;
        private System.Windows.Forms.ListBox lsbOAuth;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtIssuer;
        private System.Windows.Forms.TextBox txtSymmSecret;
    }
}
