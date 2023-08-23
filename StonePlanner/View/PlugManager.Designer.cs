namespace StonePlanner.View
{
    partial class PlugManager
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlugManager));
            this.metroLabel_Scanned = new MetroFramework.Controls.MetroLabel();
            this.listBox_Scanned = new System.Windows.Forms.ListBox();
            this.metroPanel_PlugDetails = new MetroFramework.Controls.MetroPanel();
            this.metroButton_Submit = new MetroFramework.Controls.MetroButton();
            this.pictureBox_Pict = new System.Windows.Forms.PictureBox();
            this.metroLabel_PluginVersion = new MetroFramework.Controls.MetroLabel();
            this.metroLabel_PluginAuthor = new MetroFramework.Controls.MetroLabel();
            this.metroLabel_PluginDescription = new MetroFramework.Controls.MetroLabel();
            this.metroLabel_PluginName = new MetroFramework.Controls.MetroLabel();
            this.metroPanel_PlugDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Pict)).BeginInit();
            this.SuspendLayout();
            // 
            // metroLabel_Scanned
            // 
            this.metroLabel_Scanned.AutoSize = true;
            this.metroLabel_Scanned.Location = new System.Drawing.Point(23, 77);
            this.metroLabel_Scanned.Name = "metroLabel_Scanned";
            this.metroLabel_Scanned.Size = new System.Drawing.Size(99, 20);
            this.metroLabel_Scanned.TabIndex = 0;
            this.metroLabel_Scanned.Text = "已扫描的插件";
            // 
            // listBox_Scanned
            // 
            this.listBox_Scanned.FormattingEnabled = true;
            this.listBox_Scanned.ItemHeight = 12;
            this.listBox_Scanned.Location = new System.Drawing.Point(23, 111);
            this.listBox_Scanned.Name = "listBox_Scanned";
            this.listBox_Scanned.Size = new System.Drawing.Size(194, 256);
            this.listBox_Scanned.TabIndex = 1;
            this.listBox_Scanned.SelectedIndexChanged += new System.EventHandler(this.listBox_Scanned_SelectedIndexChanged);
            // 
            // metroPanel_PlugDetails
            // 
            this.metroPanel_PlugDetails.Controls.Add(this.metroButton_Submit);
            this.metroPanel_PlugDetails.Controls.Add(this.pictureBox_Pict);
            this.metroPanel_PlugDetails.Controls.Add(this.metroLabel_PluginVersion);
            this.metroPanel_PlugDetails.Controls.Add(this.metroLabel_PluginAuthor);
            this.metroPanel_PlugDetails.Controls.Add(this.metroLabel_PluginDescription);
            this.metroPanel_PlugDetails.Controls.Add(this.metroLabel_PluginName);
            this.metroPanel_PlugDetails.HorizontalScrollbarBarColor = true;
            this.metroPanel_PlugDetails.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel_PlugDetails.HorizontalScrollbarSize = 10;
            this.metroPanel_PlugDetails.Location = new System.Drawing.Point(239, 111);
            this.metroPanel_PlugDetails.Name = "metroPanel_PlugDetails";
            this.metroPanel_PlugDetails.Size = new System.Drawing.Size(300, 256);
            this.metroPanel_PlugDetails.TabIndex = 2;
            this.metroPanel_PlugDetails.VerticalScrollbarBarColor = true;
            this.metroPanel_PlugDetails.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel_PlugDetails.VerticalScrollbarSize = 10;
            // 
            // metroButton_Submit
            // 
            this.metroButton_Submit.Location = new System.Drawing.Point(17, 217);
            this.metroButton_Submit.Name = "metroButton_Submit";
            this.metroButton_Submit.Size = new System.Drawing.Size(271, 27);
            this.metroButton_Submit.TabIndex = 7;
            this.metroButton_Submit.Text = "启用/禁用";
            this.metroButton_Submit.UseSelectable = true;
            this.metroButton_Submit.Click += new System.EventHandler(this.metroButton_Submit_Click);
            // 
            // pictureBox_Pict
            // 
            this.pictureBox_Pict.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox_Pict.BackgroundImage")));
            this.pictureBox_Pict.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Pict.Location = new System.Drawing.Point(115, 12);
            this.pictureBox_Pict.Name = "pictureBox_Pict";
            this.pictureBox_Pict.Size = new System.Drawing.Size(64, 59);
            this.pictureBox_Pict.TabIndex = 6;
            this.pictureBox_Pict.TabStop = false;
            // 
            // metroLabel_PluginVersion
            // 
            this.metroLabel_PluginVersion.AutoSize = true;
            this.metroLabel_PluginVersion.Location = new System.Drawing.Point(13, 184);
            this.metroLabel_PluginVersion.Name = "metroLabel_PluginVersion";
            this.metroLabel_PluginVersion.Size = new System.Drawing.Size(84, 20);
            this.metroLabel_PluginVersion.TabIndex = 5;
            this.metroLabel_PluginVersion.Text = "插件版本：";
            // 
            // metroLabel_PluginAuthor
            // 
            this.metroLabel_PluginAuthor.AutoSize = true;
            this.metroLabel_PluginAuthor.Location = new System.Drawing.Point(13, 152);
            this.metroLabel_PluginAuthor.Name = "metroLabel_PluginAuthor";
            this.metroLabel_PluginAuthor.Size = new System.Drawing.Size(84, 20);
            this.metroLabel_PluginAuthor.TabIndex = 4;
            this.metroLabel_PluginAuthor.Text = "插件作者：";
            // 
            // metroLabel_PluginDescription
            // 
            this.metroLabel_PluginDescription.AutoSize = true;
            this.metroLabel_PluginDescription.Location = new System.Drawing.Point(13, 120);
            this.metroLabel_PluginDescription.Name = "metroLabel_PluginDescription";
            this.metroLabel_PluginDescription.Size = new System.Drawing.Size(84, 20);
            this.metroLabel_PluginDescription.TabIndex = 3;
            this.metroLabel_PluginDescription.Text = "插件简介：";
            // 
            // metroLabel_PluginName
            // 
            this.metroLabel_PluginName.AutoSize = true;
            this.metroLabel_PluginName.Location = new System.Drawing.Point(13, 88);
            this.metroLabel_PluginName.Name = "metroLabel_PluginName";
            this.metroLabel_PluginName.Size = new System.Drawing.Size(84, 20);
            this.metroLabel_PluginName.TabIndex = 2;
            this.metroLabel_PluginName.Text = "插件名称：";
            // 
            // PlugManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 396);
            this.Controls.Add(this.metroPanel_PlugDetails);
            this.Controls.Add(this.listBox_Scanned);
            this.Controls.Add(this.metroLabel_Scanned);
            this.Name = "PlugManager";
            this.Text = "插件管理";
            this.Load += new System.EventHandler(this.PlugManager_Load);
            this.metroPanel_PlugDetails.ResumeLayout(false);
            this.metroPanel_PlugDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Pict)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel_Scanned;
        private System.Windows.Forms.ListBox listBox_Scanned;
        private MetroFramework.Controls.MetroPanel metroPanel_PlugDetails;
        private System.Windows.Forms.PictureBox pictureBox_Pict;
        private MetroFramework.Controls.MetroLabel metroLabel_PluginVersion;
        private MetroFramework.Controls.MetroLabel metroLabel_PluginAuthor;
        private MetroFramework.Controls.MetroLabel metroLabel_PluginDescription;
        private MetroFramework.Controls.MetroLabel metroLabel_PluginName;
        private MetroFramework.Controls.MetroButton metroButton_Submit;
    }
}