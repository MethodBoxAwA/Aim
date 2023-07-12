using System;
using MetroFramework.Forms;
using System.Collections.Generic;
using System.Windows.Forms;

namespace StonePlanner
{
    public partial class Alert : MetroForm
    {
        // task list
        List<string> tasks;

        /// <summary>
        /// alert initialize function
        /// </summary>
        /// <param name="tasks"></param>
        /// <param name="alert"></param>
        public Alert(List<string> tasks,string alert = "")
        {
            InitializeComponent();
            this.tasks = tasks;
            // cannot get any tasks
            if (alert == "")
            {
                ErrorCenter.AddError(DataType.ExceptionsLevel.Caution, 
                    new Exceptions.UnknownException("排班日历发生故障，应尽快修复"));
            }
            // display result
            metroLabel_WorkAlert.Text = $"排班日历：{alert}";
        }

        /// <summary>
        /// close window
        /// </summary>
        private void button_S_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// scan all tasks
        /// </summary>
        private void Alert_Load(object sender, EventArgs e)
        {
            // scan all tasks
            foreach (var item in tasks)
            {
                listBox_Task.Items.Add(item);
            }
        }
    }
}
