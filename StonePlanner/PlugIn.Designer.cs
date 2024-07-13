namespace StonePlanner
{
    partial class PlugIn
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
            this.listBox_Plugs = new System.Windows.Forms.ListBox();
            this.metroLabel_Plug = new MetroFramework.Controls.MetroLabel();
            this.panel_Details = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // listBox_Plugs
            // 
            this.listBox_Plugs.FormattingEnabled = true;
            this.listBox_Plugs.ItemHeight = 12;
            this.listBox_Plugs.Location = new System.Drawing.Point(41, 114);
            this.listBox_Plugs.Name = "listBox_Plugs";
            this.listBox_Plugs.Size = new System.Drawing.Size(315, 304);
            this.listBox_Plugs.TabIndex = 0;
            this.listBox_Plugs.SelectedIndexChanged += new System.EventHandler(this.listBox_Plugs_SelectedIndexChanged);
            // 
            // metroLabel_Plug
            // 
            this.metroLabel_Plug.AutoSize = true;
            this.metroLabel_Plug.FontSize = MetroFramework.MetroLabelSize.Tall;
            this.metroLabel_Plug.FontWeight = MetroFramework.MetroLabelWeight.Regular;
            this.metroLabel_Plug.Location = new System.Drawing.Point(35, 72);
            this.metroLabel_Plug.Name = "metroLabel_Plug";
            this.metroLabel_Plug.Size = new System.Drawing.Size(107, 25);
            this.metroLabel_Plug.TabIndex = 1;
            this.metroLabel_Plug.Text = "插件列表：";
            // 
            // panel_Details
            // 
            this.panel_Details.Location = new System.Drawing.Point(389, 63);
            this.panel_Details.Name = "panel_Details";
            this.panel_Details.Size = new System.Drawing.Size(388, 355);
            this.panel_Details.TabIndex = 2;
            // 
            // PlugIn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel_Details);
            this.Controls.Add(this.metroLabel_Plug);
            this.Controls.Add(this.listBox_Plugs);
            this.Name = "PlugIn";
            this.Text = "PlugIn";
            this.Load += new System.EventHandler(this.PlugIn_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_Plugs;
        private MetroFramework.Controls.MetroLabel metroLabel_Plug;
        private System.Windows.Forms.Panel panel_Details;
    }
}