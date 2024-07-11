using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using static StonePlanner.DataType.Structs;

namespace StonePlanner
{
    public partial class AddList : MetroForm
    {
        public AddList()
        {
            InitializeComponent();
        }

        private void button_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                // Build entity
                var entity = AccessEntity.GetAccessEntityInstance();
                var taskList = new TaskList { ListName = textBox_Listname.Text };

                // Insert to database
                entity.AddElement(taskList, "tb_Lists", new List<string> { "ID" });
            }
            catch(Exception ex) 
            {
                ErrorCenter.AddError(DataType.ExceptionsLevel.Warning, ex);
                MessageBox.Show($"添加失败，原因是{ex.Message}。","失败",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        private void AddList_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }
    }
}
