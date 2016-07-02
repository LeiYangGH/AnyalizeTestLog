namespace LogProcessorWinForm
{
    partial class FrmMain
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
            this.lvwTests = new System.Windows.Forms.ListView();
            this.Item = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SN = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblLogFileName = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnExtractTests = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelCtrl = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLblMsg = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelCtrl.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwTests
            // 
            this.lvwTests.CheckBoxes = true;
            this.lvwTests.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Item,
            this.Date,
            this.Status,
            this.SN});
            this.lvwTests.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTests.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwTests.Location = new System.Drawing.Point(0, 134);
            this.lvwTests.Margin = new System.Windows.Forms.Padding(4);
            this.lvwTests.Name = "lvwTests";
            this.lvwTests.ShowItemToolTips = true;
            this.lvwTests.Size = new System.Drawing.Size(687, 353);
            this.lvwTests.TabIndex = 0;
            this.lvwTests.UseCompatibleStateImageBehavior = false;
            this.lvwTests.View = System.Windows.Forms.View.Details;
            this.lvwTests.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvwTests_ItemChecked);
            // 
            // Item
            // 
            this.Item.Text = "Item";
            // 
            // Date
            // 
            this.Date.Text = "Date";
            this.Date.Width = 150;
            // 
            // Status
            // 
            this.Status.Text = "Status";
            // 
            // SN
            // 
            this.SN.Text = "SN";
            this.SN.Width = 120;
            // 
            // lblLogFileName
            // 
            this.lblLogFileName.AutoSize = true;
            this.lblLogFileName.Location = new System.Drawing.Point(195, 21);
            this.lblLogFileName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblLogFileName.Name = "lblLogFileName";
            this.lblLogFileName.Size = new System.Drawing.Size(121, 25);
            this.lblLogFileName.TabIndex = 1;
            this.lblLogFileName.Text = "log file name";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(31, 13);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(141, 41);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "Open";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnExtractTests
            // 
            this.btnExtractTests.Location = new System.Drawing.Point(31, 75);
            this.btnExtractTests.Margin = new System.Windows.Forms.Padding(4);
            this.btnExtractTests.Name = "btnExtractTests";
            this.btnExtractTests.Size = new System.Drawing.Size(141, 41);
            this.btnExtractTests.TabIndex = 3;
            this.btnExtractTests.Text = "ExtractTests";
            this.btnExtractTests.UseVisualStyleBackColor = true;
            this.btnExtractTests.Click += new System.EventHandler(this.btnExtractTests_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(498, 75);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(141, 41);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panelCtrl
            // 
            this.panelCtrl.Controls.Add(this.lblLogFileName);
            this.panelCtrl.Controls.Add(this.btnBrowse);
            this.panelCtrl.Controls.Add(this.btnExtractTests);
            this.panelCtrl.Controls.Add(this.btnSave);
            this.panelCtrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelCtrl.Location = new System.Drawing.Point(0, 0);
            this.panelCtrl.Margin = new System.Windows.Forms.Padding(4);
            this.panelCtrl.Name = "panelCtrl";
            this.panelCtrl.Size = new System.Drawing.Size(687, 134);
            this.panelCtrl.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusLblMsg});
            this.statusStrip1.Location = new System.Drawing.Point(0, 487);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(687, 36);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 30);
            // 
            // statusLblMsg
            // 
            this.statusLblMsg.Name = "statusLblMsg";
            this.statusLblMsg.Size = new System.Drawing.Size(82, 31);
            this.statusLblMsg.Text = "message";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 523);
            this.Controls.Add(this.lvwTests);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panelCtrl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmMain";
            this.Text = "Log Extractor";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.panelCtrl.ResumeLayout(false);
            this.panelCtrl.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwTests;
        private System.Windows.Forms.ColumnHeader Date;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.Label lblLogFileName;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnExtractTests;
        private System.Windows.Forms.ColumnHeader Item;
        private System.Windows.Forms.ColumnHeader SN;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panelCtrl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLblMsg;
    }
}

