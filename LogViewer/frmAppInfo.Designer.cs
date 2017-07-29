namespace LogViewer
{
    partial class frmAppInfo
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tp1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblProcessorCount = new System.Windows.Forms.Label();
            this.lblOpenIZVersion = new System.Windows.Forms.Label();
            this.lblOpenIZInfoVersion = new System.Windows.Forms.Label();
            this.lblSku = new System.Windows.Forms.Label();
            this.lblOperatingSystem = new System.Windows.Forms.Label();
            this.lbl64Bit = new System.Windows.Forms.Label();
            this.lblUsedMemory = new System.Windows.Forms.Label();
            this.lblNetVersion = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tp2 = new System.Windows.Forms.TabPage();
            this.lvAsm = new System.Windows.Forms.ListView();
            this.colAsm = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVer = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tp3 = new System.Windows.Forms.TabPage();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tp4 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lvSync = new System.Windows.Forms.ListView();
            this.colItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFilter = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabControl1.SuspendLayout();
            this.tp1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tp2.SuspendLayout();
            this.tp3.SuspendLayout();
            this.tp4.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tp1);
            this.tabControl1.Controls.Add(this.tp2);
            this.tabControl1.Controls.Add(this.tp3);
            this.tabControl1.Controls.Add(this.tp4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(570, 435);
            this.tabControl1.TabIndex = 0;
            // 
            // tp1
            // 
            this.tp1.Controls.Add(this.tableLayoutPanel1);
            this.tp1.Location = new System.Drawing.Point(4, 29);
            this.tp1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp1.Name = "tp1";
            this.tp1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp1.Size = new System.Drawing.Size(562, 402);
            this.tp1.TabIndex = 0;
            this.tp1.Text = "OpenIZ Version";
            this.tp1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.34091F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.65909F));
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lblProcessorCount, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lblOpenIZVersion, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblOpenIZInfoVersion, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblSku, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblOperatingSystem, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbl64Bit, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblUsedMemory, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.lblNetVersion, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 5);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(554, 392);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 343);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(215, 49);
            this.label4.TabIndex = 21;
            this.label4.Text = "CPU Count";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblProcessorCount
            // 
            this.lblProcessorCount.AutoSize = true;
            this.lblProcessorCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblProcessorCount.Location = new System.Drawing.Point(227, 343);
            this.lblProcessorCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcessorCount.Name = "lblProcessorCount";
            this.lblProcessorCount.Size = new System.Drawing.Size(323, 49);
            this.lblProcessorCount.TabIndex = 20;
            this.lblProcessorCount.Text = "1";
            this.lblProcessorCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOpenIZVersion
            // 
            this.lblOpenIZVersion.AutoSize = true;
            this.lblOpenIZVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenIZVersion.Location = new System.Drawing.Point(227, 0);
            this.lblOpenIZVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOpenIZVersion.Name = "lblOpenIZVersion";
            this.lblOpenIZVersion.Size = new System.Drawing.Size(323, 49);
            this.lblOpenIZVersion.TabIndex = 19;
            this.lblOpenIZVersion.Text = "v0.0.0.0";
            this.lblOpenIZVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOpenIZInfoVersion
            // 
            this.lblOpenIZInfoVersion.AutoSize = true;
            this.lblOpenIZInfoVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpenIZInfoVersion.Location = new System.Drawing.Point(227, 49);
            this.lblOpenIZInfoVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOpenIZInfoVersion.Name = "lblOpenIZInfoVersion";
            this.lblOpenIZInfoVersion.Size = new System.Drawing.Size(323, 49);
            this.lblOpenIZInfoVersion.TabIndex = 18;
            this.lblOpenIZInfoVersion.Text = "No Version Name";
            this.lblOpenIZInfoVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSku
            // 
            this.lblSku.AutoSize = true;
            this.lblSku.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSku.Location = new System.Drawing.Point(227, 98);
            this.lblSku.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSku.Name = "lblSku";
            this.lblSku.Size = new System.Drawing.Size(323, 49);
            this.lblSku.TabIndex = 17;
            this.lblSku.Text = "SKU";
            this.lblSku.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblOperatingSystem
            // 
            this.lblOperatingSystem.AutoSize = true;
            this.lblOperatingSystem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOperatingSystem.Location = new System.Drawing.Point(227, 147);
            this.lblOperatingSystem.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOperatingSystem.Name = "lblOperatingSystem";
            this.lblOperatingSystem.Size = new System.Drawing.Size(323, 49);
            this.lblOperatingSystem.TabIndex = 16;
            this.lblOperatingSystem.Text = "OS";
            this.lblOperatingSystem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbl64Bit
            // 
            this.lbl64Bit.AutoSize = true;
            this.lbl64Bit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl64Bit.Location = new System.Drawing.Point(227, 196);
            this.lbl64Bit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl64Bit.Name = "lbl64Bit";
            this.lbl64Bit.Size = new System.Drawing.Size(323, 49);
            this.lbl64Bit.TabIndex = 15;
            this.lbl64Bit.Text = "No";
            this.lbl64Bit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUsedMemory
            // 
            this.lblUsedMemory.AutoSize = true;
            this.lblUsedMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUsedMemory.Location = new System.Drawing.Point(227, 245);
            this.lblUsedMemory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUsedMemory.Name = "lblUsedMemory";
            this.lblUsedMemory.Size = new System.Drawing.Size(323, 49);
            this.lblUsedMemory.TabIndex = 14;
            this.lblUsedMemory.Text = "1,203 kb";
            this.lblUsedMemory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblNetVersion
            // 
            this.lblNetVersion.AutoSize = true;
            this.lblNetVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNetVersion.Location = new System.Drawing.Point(227, 294);
            this.lblNetVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNetVersion.Name = "lblNetVersion";
            this.lblNetVersion.Size = new System.Drawing.Size(323, 49);
            this.lblNetVersion.TabIndex = 13;
            this.lblNetVersion.Text = "v4.0";
            this.lblNetVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(4, 294);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(215, 49);
            this.label13.TabIndex = 12;
            this.label13.Text = ".NET Version";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(4, 245);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(215, 49);
            this.label11.TabIndex = 10;
            this.label11.Text = "Used Memory";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(4, 196);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 49);
            this.label9.TabIndex = 8;
            this.label9.Text = "Is 64 Bit?";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 147);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(215, 49);
            this.label7.TabIndex = 6;
            this.label7.Text = "Operating System";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(4, 98);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(215, 49);
            this.label5.TabIndex = 4;
            this.label5.Text = "Product SKU";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(4, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(215, 49);
            this.label3.TabIndex = 2;
            this.label3.Text = "Informational Version";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 49);
            this.label1.TabIndex = 0;
            this.label1.Text = "OpenIZ Core Version";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tp2
            // 
            this.tp2.Controls.Add(this.lvAsm);
            this.tp2.Location = new System.Drawing.Point(4, 29);
            this.tp2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp2.Name = "tp2";
            this.tp2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp2.Size = new System.Drawing.Size(562, 402);
            this.tp2.TabIndex = 1;
            this.tp2.Text = "Assemblies";
            this.tp2.UseVisualStyleBackColor = true;
            // 
            // lvAsm
            // 
            this.lvAsm.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAsm,
            this.colVer,
            this.colInfo});
            this.lvAsm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAsm.FullRowSelect = true;
            this.lvAsm.Location = new System.Drawing.Point(4, 5);
            this.lvAsm.Name = "lvAsm";
            this.lvAsm.Size = new System.Drawing.Size(554, 392);
            this.lvAsm.TabIndex = 0;
            this.lvAsm.UseCompatibleStateImageBehavior = false;
            this.lvAsm.View = System.Windows.Forms.View.Details;
            // 
            // colAsm
            // 
            this.colAsm.Text = "Assembly";
            this.colAsm.Width = 120;
            // 
            // colVer
            // 
            this.colVer.Text = "Version";
            this.colVer.Width = 120;
            // 
            // colInfo
            // 
            this.colInfo.Text = "Description";
            this.colInfo.Width = 120;
            // 
            // tp3
            // 
            this.tp3.Controls.Add(this.lvFiles);
            this.tp3.Location = new System.Drawing.Point(4, 29);
            this.tp3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp3.Name = "tp3";
            this.tp3.Size = new System.Drawing.Size(562, 402);
            this.tp3.TabIndex = 2;
            this.tp3.Text = "Files";
            this.tp3.UseVisualStyleBackColor = true;
            // 
            // lvFiles
            // 
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colFile,
            this.colSize});
            this.lvFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFiles.FullRowSelect = true;
            this.lvFiles.Location = new System.Drawing.Point(0, 0);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(562, 402);
            this.lvFiles.TabIndex = 0;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // colFile
            // 
            this.colFile.Text = "File";
            this.colFile.Width = 200;
            // 
            // colSize
            // 
            this.colSize.Text = "Size";
            this.colSize.Width = 200;
            // 
            // tp4
            // 
            this.tp4.Controls.Add(this.lvSync);
            this.tp4.Location = new System.Drawing.Point(4, 29);
            this.tp4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tp4.Name = "tp4";
            this.tp4.Size = new System.Drawing.Size(562, 402);
            this.tp4.TabIndex = 3;
            this.tp4.Text = "Synchronization";
            this.tp4.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnClose);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 435);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(570, 48);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(454, 5);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(112, 35);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lvSync
            // 
            this.lvSync.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colItem,
            this.colDate,
            this.colVersion,
            this.colFilter});
            this.lvSync.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSync.Location = new System.Drawing.Point(0, 0);
            this.lvSync.Name = "lvSync";
            this.lvSync.Size = new System.Drawing.Size(562, 402);
            this.lvSync.TabIndex = 0;
            this.lvSync.UseCompatibleStateImageBehavior = false;
            this.lvSync.View = System.Windows.Forms.View.Details;
            // 
            // colItem
            // 
            this.colItem.Text = "Sync Item";
            this.colItem.Width = 133;
            // 
            // colDate
            // 
            this.colDate.Text = "Last Sync";
            this.colDate.Width = 145;
            // 
            // colVersion
            // 
            this.colVersion.Text = "Last Version";
            this.colVersion.Width = 138;
            // 
            // colFilter
            // 
            this.colFilter.Text = "Filter";
            this.colFilter.Width = 339;
            // 
            // frmAppInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 483);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmAppInfo";
            this.Text = "View AppInfo File";
            this.tabControl1.ResumeLayout(false);
            this.tp1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tp2.ResumeLayout(false);
            this.tp3.ResumeLayout(false);
            this.tp4.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tp1;
        private System.Windows.Forms.TabPage tp2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tp3;
        private System.Windows.Forms.TabPage tp4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblOpenIZVersion;
        private System.Windows.Forms.Label lblOpenIZInfoVersion;
        private System.Windows.Forms.Label lblSku;
        private System.Windows.Forms.Label lblOperatingSystem;
        private System.Windows.Forms.Label lbl64Bit;
        private System.Windows.Forms.Label lblUsedMemory;
        private System.Windows.Forms.Label lblNetVersion;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblProcessorCount;
        private System.Windows.Forms.ListView lvAsm;
        private System.Windows.Forms.ColumnHeader colAsm;
        private System.Windows.Forms.ColumnHeader colVer;
        private System.Windows.Forms.ColumnHeader colInfo;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ListView lvSync;
        private System.Windows.Forms.ColumnHeader colItem;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ColumnHeader colVersion;
        private System.Windows.Forms.ColumnHeader colFilter;
    }
}