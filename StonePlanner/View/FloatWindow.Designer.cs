﻿namespace StonePlanner
{
    partial class FloatWindow
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
            this.panel_Only = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel_Only
            // 
            this.panel_Only.Location = new System.Drawing.Point(1, -1);
            this.panel_Only.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_Only.Name = "panel_Only";
            this.panel_Only.Size = new System.Drawing.Size(178, 30);
            this.panel_Only.TabIndex = 0;
            this.panel_Only.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Only_Paint);
            // 
            // FloatWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(180, 27);
            this.Controls.Add(this.panel_Only);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "FloatWindow";
            this.Text = "FloatWindow";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Only;
    }
}