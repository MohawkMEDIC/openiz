namespace LogViewer
{
    partial class frmDataInspector
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbxViewer = new System.Windows.Forms.ComboBox();
            this.txtDecode = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "View As:";
            // 
            // cbxViewer
            // 
            this.cbxViewer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxViewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxViewer.FormattingEnabled = true;
            this.cbxViewer.Location = new System.Drawing.Point(66, 6);
            this.cbxViewer.Name = "cbxViewer";
            this.cbxViewer.Size = new System.Drawing.Size(419, 21);
            this.cbxViewer.TabIndex = 1;
            this.cbxViewer.SelectedIndexChanged += new System.EventHandler(this.cbxViewer_SelectedIndexChanged);
            // 
            // txtDecode
            // 
            this.txtDecode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDecode.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDecode.Location = new System.Drawing.Point(12, 33);
            this.txtDecode.Multiline = true;
            this.txtDecode.Name = "txtDecode";
            this.txtDecode.ReadOnly = true;
            this.txtDecode.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDecode.Size = new System.Drawing.Size(473, 374);
            this.txtDecode.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(410, 413);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmDataInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 444);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtDecode);
            this.Controls.Add(this.cbxViewer);
            this.Controls.Add(this.label1);
            this.Name = "frmDataInspector";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Data Inspector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxViewer;
        private System.Windows.Forms.TextBox txtDecode;
        private System.Windows.Forms.Button btnClose;
    }
}