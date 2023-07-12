using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// export all unfinished task (Deprecated)
    /// </summary>
    public partial class ExportTodo : Form
    {
        /// <summary>
        /// all plan control
        /// </summary>
        System.Windows.Forms.Control.ControlCollection planCollcetion;
        /// <summary>
        /// initialize component and load collection
        /// </summary>
        /// <param name="szPlanCollcetions">plan control collection</param>
        public ExportTodo(Control.ControlCollection szPlanCollcetions)
        {
            InitializeComponent();

            this.planCollcetion = szPlanCollcetions;
        }

        /// <summary>
        /// load plan information and export as texture
        /// </summary>
        private void ExportTodo_Load(object sender, EventArgs e)
        {
            try
            {
                label_T.Text = "导出您的待办";
                richTextBox_M.Text +=
                     $"序号    标题                状态\n";
                List<string> planCapital = new List<string>();
                List<string> planStatus = new List<string>();
                int i = 0;
                planCollcetion.RemoveAt(0);
                planCollcetion.RemoveAt(0);
                foreach (Plan item in planCollcetion)
                {
                    if (item.GetType() is null) { continue; }
                    richTextBox_M.Text +=
                        $"{i}    {item.capital}          {item.status}\n";
                    planCapital.Add(item.capital);
                    planStatus.Add(item.status);
                    i++;
                }
                // free
                i = 0;
            }
            catch 
            { 
                // have no plan
                MessageBox.Show("请先添加待办！");
            }
        }

        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
