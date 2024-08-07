﻿using System;
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
    public partial class ExportTodo : Form
    {
        System.Windows.Forms.Control.ControlCollection planCollcetion;
        public ExportTodo(Control.ControlCollection szPlanCollcetions)
        {
            InitializeComponent();

            this.planCollcetion = szPlanCollcetions;
        }

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
                        $"{i}    {item.Capital}          {item.Status}\n";
                    planCapital.Add(item.Capital);
                    planStatus.Add(item.Status);
                    i++;
                }
                //释放
                i = 0;
            }
            catch { MessageBox.Show("请先添加待办！");}
        }

        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }
    }
}
