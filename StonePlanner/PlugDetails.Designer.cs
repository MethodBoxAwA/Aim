namespace StonePlanner
{
    partial class PlugDetails
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label_PlugCaptial = new System.Windows.Forms.Label();
            this.label_PlugContent = new System.Windows.Forms.Label();
            this.label_DeveloperContent = new System.Windows.Forms.Label();
            this.label_DeveloperCaptial = new System.Windows.Forms.Label();
            this.label_IntroduceContent = new System.Windows.Forms.Label();
            this.label_IntroduceCaptial = new System.Windows.Forms.Label();
            this.metroButton_Disable = new MetroFramework.Controls.MetroButton();
            this.metroButton_Enable = new MetroFramework.Controls.MetroButton();
            this.label_StatusContent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_PlugCaptial
            // 
            this.label_PlugCaptial.AutoSize = true;
            this.label_PlugCaptial.Font = new System.Drawing.Font("黑体", 13F);
            this.label_PlugCaptial.Location = new System.Drawing.Point(27, 35);
            this.label_PlugCaptial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_PlugCaptial.Name = "label_PlugCaptial";
            this.label_PlugCaptial.Size = new System.Drawing.Size(98, 22);
            this.label_PlugCaptial.TabIndex = 0;
            this.label_PlugCaptial.Text = "插件名：";
            // 
            // label_PlugContent
            // 
            this.label_PlugContent.AutoSize = true;
            this.label_PlugContent.Font = new System.Drawing.Font("黑体", 13F);
            this.label_PlugContent.Location = new System.Drawing.Point(117, 35);
            this.label_PlugContent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_PlugContent.Name = "label_PlugContent";
            this.label_PlugContent.Size = new System.Drawing.Size(76, 22);
            this.label_PlugContent.TabIndex = 1;
            this.label_PlugContent.Text = "插件名";
            // 
            // label_DeveloperContent
            // 
            this.label_DeveloperContent.AutoSize = true;
            this.label_DeveloperContent.Font = new System.Drawing.Font("黑体", 13F);
            this.label_DeveloperContent.Location = new System.Drawing.Point(117, 86);
            this.label_DeveloperContent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_DeveloperContent.Name = "label_DeveloperContent";
            this.label_DeveloperContent.Size = new System.Drawing.Size(76, 22);
            this.label_DeveloperContent.TabIndex = 3;
            this.label_DeveloperContent.Text = "开发者";
            // 
            // label_DeveloperCaptial
            // 
            this.label_DeveloperCaptial.AutoSize = true;
            this.label_DeveloperCaptial.Font = new System.Drawing.Font("黑体", 13F);
            this.label_DeveloperCaptial.Location = new System.Drawing.Point(27, 85);
            this.label_DeveloperCaptial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_DeveloperCaptial.Name = "label_DeveloperCaptial";
            this.label_DeveloperCaptial.Size = new System.Drawing.Size(98, 22);
            this.label_DeveloperCaptial.TabIndex = 2;
            this.label_DeveloperCaptial.Text = "开发者：";
            // 
            // label_IntroduceContent
            // 
            this.label_IntroduceContent.AutoSize = true;
            this.label_IntroduceContent.Font = new System.Drawing.Font("黑体", 13F);
            this.label_IntroduceContent.Location = new System.Drawing.Point(117, 136);
            this.label_IntroduceContent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IntroduceContent.Name = "label_IntroduceContent";
            this.label_IntroduceContent.Size = new System.Drawing.Size(76, 22);
            this.label_IntroduceContent.TabIndex = 5;
            this.label_IntroduceContent.Text = "简  介";
            // 
            // label_IntroduceCaptial
            // 
            this.label_IntroduceCaptial.AutoSize = true;
            this.label_IntroduceCaptial.Font = new System.Drawing.Font("黑体", 13F);
            this.label_IntroduceCaptial.Location = new System.Drawing.Point(27, 135);
            this.label_IntroduceCaptial.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_IntroduceCaptial.Name = "label_IntroduceCaptial";
            this.label_IntroduceCaptial.Size = new System.Drawing.Size(98, 22);
            this.label_IntroduceCaptial.TabIndex = 4;
            this.label_IntroduceCaptial.Text = "简  介：";
            // 
            // metroButton_Disable
            // 
            this.metroButton_Disable.Location = new System.Drawing.Point(65, 388);
            this.metroButton_Disable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metroButton_Disable.Name = "metroButton_Disable";
            this.metroButton_Disable.Size = new System.Drawing.Size(159, 44);
            this.metroButton_Disable.TabIndex = 6;
            this.metroButton_Disable.Text = "禁用插件";
            this.metroButton_Disable.UseSelectable = true;
            this.metroButton_Disable.Click += new System.EventHandler(this.metroButton_Enable_Click);
            // 
            // metroButton_Enable
            // 
            this.metroButton_Enable.Location = new System.Drawing.Point(277, 388);
            this.metroButton_Enable.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.metroButton_Enable.Name = "metroButton_Enable";
            this.metroButton_Enable.Size = new System.Drawing.Size(159, 44);
            this.metroButton_Enable.TabIndex = 7;
            this.metroButton_Enable.Text = "启用插件";
            this.metroButton_Enable.UseSelectable = true;
            this.metroButton_Enable.Click += new System.EventHandler(this.metroButton_Enable_Click);
            // 
            // label_StatusContent
            // 
            this.label_StatusContent.AutoSize = true;
            this.label_StatusContent.Font = new System.Drawing.Font("黑体", 10F);
            this.label_StatusContent.Location = new System.Drawing.Point(173, 358);
            this.label_StatusContent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_StatusContent.Name = "label_StatusContent";
            this.label_StatusContent.Size = new System.Drawing.Size(152, 17);
            this.label_StatusContent.TabIndex = 8;
            this.label_StatusContent.Text = "当前状态：已启用";
            // 
            // PlugDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_StatusContent);
            this.Controls.Add(this.metroButton_Enable);
            this.Controls.Add(this.metroButton_Disable);
            this.Controls.Add(this.label_IntroduceContent);
            this.Controls.Add(this.label_IntroduceCaptial);
            this.Controls.Add(this.label_DeveloperContent);
            this.Controls.Add(this.label_DeveloperCaptial);
            this.Controls.Add(this.label_PlugContent);
            this.Controls.Add(this.label_PlugCaptial);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "PlugDetails";
            this.Size = new System.Drawing.Size(517, 444);
            this.Load += new System.EventHandler(this.PlugDetails_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_PlugCaptial;
        private System.Windows.Forms.Label label_PlugContent;
        private System.Windows.Forms.Label label_DeveloperContent;
        private System.Windows.Forms.Label label_DeveloperCaptial;
        private System.Windows.Forms.Label label_IntroduceContent;
        private System.Windows.Forms.Label label_IntroduceCaptial;
        private MetroFramework.Controls.MetroButton metroButton_Disable;
        private MetroFramework.Controls.MetroButton metroButton_Enable;
        private System.Windows.Forms.Label label_StatusContent;
    }
}
