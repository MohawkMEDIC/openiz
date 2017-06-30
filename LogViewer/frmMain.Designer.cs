namespace LogViewer
{
    partial class frmMain
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.trvSource = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.lsvEvents = new System.Windows.Forms.ListView();
            this.colId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSource = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colThread = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colText = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showCousinsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showNeighboursOnSameThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.sameSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sameThreadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sameLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtSearch = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.cbxLevel = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.cbxThread = new System.Windows.Forms.ToolStripComboBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.decodeBase64ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtText = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(1071, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 22);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(152, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "&Exit";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tspProgress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 490);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 9, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1071, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(992, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Status...";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tspProgress
            // 
            this.tspProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspProgress.Name = "tspProgress";
            this.tspProgress.Size = new System.Drawing.Size(67, 16);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.trvSource);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Size = new System.Drawing.Size(1071, 466);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 3;
            // 
            // trvSource
            // 
            this.trvSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvSource.Location = new System.Drawing.Point(0, 0);
            this.trvSource.Margin = new System.Windows.Forms.Padding(2);
            this.trvSource.Name = "trvSource";
            this.trvSource.Size = new System.Drawing.Size(199, 466);
            this.trvSource.TabIndex = 0;
            this.trvSource.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvSource_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 25);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.lsvEvents);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtText);
            this.splitContainer2.Panel2.Controls.Add(this.label1);
            this.splitContainer2.Panel2MinSize = 200;
            this.splitContainer2.Size = new System.Drawing.Size(869, 441);
            this.splitContainer2.SplitterDistance = 238;
            this.splitContainer2.SplitterWidth = 3;
            this.splitContainer2.TabIndex = 4;
            // 
            // lsvEvents
            // 
            this.lsvEvents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colId,
            this.colLevel,
            this.colSource,
            this.colTime,
            this.colThread,
            this.colText});
            this.lsvEvents.ContextMenuStrip = this.contextMenuStrip1;
            this.lsvEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvEvents.FullRowSelect = true;
            this.lsvEvents.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lsvEvents.HideSelection = false;
            this.lsvEvents.Location = new System.Drawing.Point(0, 0);
            this.lsvEvents.Margin = new System.Windows.Forms.Padding(2);
            this.lsvEvents.Name = "lsvEvents";
            this.lsvEvents.Size = new System.Drawing.Size(869, 238);
            this.lsvEvents.TabIndex = 4;
            this.lsvEvents.UseCompatibleStateImageBehavior = false;
            this.lsvEvents.View = System.Windows.Forms.View.Details;
            this.lsvEvents.SelectedIndexChanged += new System.EventHandler(this.lsvEvents_SelectedIndexChanged);
            // 
            // colId
            // 
            this.colId.Text = "#";
            // 
            // colLevel
            // 
            this.colLevel.Text = "Level";
            this.colLevel.Width = 112;
            // 
            // colSource
            // 
            this.colSource.Text = "Source";
            this.colSource.Width = 161;
            // 
            // colTime
            // 
            this.colTime.Text = "Date/Time";
            this.colTime.Width = 120;
            // 
            // colThread
            // 
            this.colThread.Text = "Thread";
            this.colThread.Width = 166;
            // 
            // colText
            // 
            this.colText.Text = "Message";
            this.colText.Width = 502;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showCousinsToolStripMenuItem,
            this.showNeighboursOnSameThreadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.sameSourceToolStripMenuItem,
            this.sameThreadToolStripMenuItem,
            this.sameLevelToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 120);
            // 
            // showCousinsToolStripMenuItem
            // 
            this.showCousinsToolStripMenuItem.Name = "showCousinsToolStripMenuItem";
            this.showCousinsToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.showCousinsToolStripMenuItem.Text = "Show Neighbours";
            this.showCousinsToolStripMenuItem.Click += new System.EventHandler(this.showCousinsToolStripMenuItem_Click);
            // 
            // showNeighboursOnSameThreadToolStripMenuItem
            // 
            this.showNeighboursOnSameThreadToolStripMenuItem.Name = "showNeighboursOnSameThreadToolStripMenuItem";
            this.showNeighboursOnSameThreadToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.showNeighboursOnSameThreadToolStripMenuItem.Text = "Show Thread Neighbours";
            this.showNeighboursOnSameThreadToolStripMenuItem.Click += new System.EventHandler(this.showNeighboursOnSameThreadToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(205, 6);
            // 
            // sameSourceToolStripMenuItem
            // 
            this.sameSourceToolStripMenuItem.Name = "sameSourceToolStripMenuItem";
            this.sameSourceToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sameSourceToolStripMenuItem.Text = "Same Source";
            this.sameSourceToolStripMenuItem.Click += new System.EventHandler(this.sameSourceToolStripMenuItem_Click);
            // 
            // sameThreadToolStripMenuItem
            // 
            this.sameThreadToolStripMenuItem.Name = "sameThreadToolStripMenuItem";
            this.sameThreadToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sameThreadToolStripMenuItem.Text = "Same Thread";
            this.sameThreadToolStripMenuItem.Click += new System.EventHandler(this.sameThreadToolStripMenuItem_Click);
            // 
            // sameLevelToolStripMenuItem
            // 
            this.sameLevelToolStripMenuItem.Name = "sameLevelToolStripMenuItem";
            this.sameLevelToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sameLevelToolStripMenuItem.Text = "Same Level";
            this.sameLevelToolStripMenuItem.Click += new System.EventHandler(this.sameLevelToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(869, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Event Information";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.txtSearch,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.cbxLevel,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.cbxThread});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(869, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(45, 22);
            this.toolStripLabel1.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.AutoSize = false;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(135, 23);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(60, 22);
            this.toolStripLabel2.Text = "Log Level:";
            // 
            // cbxLevel
            // 
            this.cbxLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxLevel.Name = "cbxLevel";
            this.cbxLevel.Size = new System.Drawing.Size(121, 25);
            this.cbxLevel.SelectedIndexChanged += new System.EventHandler(this.cbxLevel_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(47, 22);
            this.toolStripLabel3.Text = "Thread:";
            // 
            // cbxThread
            // 
            this.cbxThread.AutoSize = false;
            this.cbxThread.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxThread.Name = "cbxThread";
            this.cbxThread.Size = new System.Drawing.Size(201, 23);
            this.cbxThread.SelectedIndexChanged += new System.EventHandler(this.cbxThread_SelectedIndexChanged);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.decodeBase64ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(149, 26);
            // 
            // decodeBase64ToolStripMenuItem
            // 
            this.decodeBase64ToolStripMenuItem.Name = "decodeBase64ToolStripMenuItem";
            this.decodeBase64ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.decodeBase64ToolStripMenuItem.Text = "Inspect Data...";
            this.decodeBase64ToolStripMenuItem.Click += new System.EventHandler(this.decodeBase64ToolStripMenuItem_Click);
            // 
            // txtText
            // 
            this.txtText.ContextMenuStrip = this.contextMenuStrip2;
            this.txtText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtText.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtText.Location = new System.Drawing.Point(0, 19);
            this.txtText.Name = "txtText";
            this.txtText.ReadOnly = true;
            this.txtText.Size = new System.Drawing.Size(869, 181);
            this.txtText.TabIndex = 1;
            this.txtText.Text = "";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1071, 512);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmMain";
            this.Text = "OpenIZ Log Viewer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar tspProgress;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView trvSource;
        private System.Windows.Forms.ListView lsvEvents;
        private System.Windows.Forms.ColumnHeader colLevel;
        private System.Windows.Forms.ColumnHeader colSource;
        private System.Windows.Forms.ColumnHeader colTime;
        private System.Windows.Forms.ColumnHeader colThread;
        private System.Windows.Forms.ColumnHeader colText;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox cbxLevel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox cbxThread;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem showCousinsToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader colId;
        private System.Windows.Forms.ToolStripMenuItem showNeighboursOnSameThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem sameSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sameThreadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sameLevelToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem decodeBase64ToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtText;
    }
}

