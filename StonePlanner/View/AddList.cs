using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using StonePlanner.Classes.DataHandlers;
using StonePlanner.Classes.DataTypes;

namespace StonePlanner.View
{
    public partial class AddList : MetroForm
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public AddList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// insert new list
        /// </summary>
        private void button_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                SQLConnect.SQLCommandExecution($"INSERT INTO Lists (ListName) VALUES('{textBox_Listname.Text}')",ref Main.odcConnection);
                Close();   
            }
            catch(Exception ex) 
            {
                // encounter insert error
                ErrorCenter.AddError(DataType.ExceptionsLevel.Warning, ex);
                MessageBox.Show($"添加失败，原因是{ex.Message}。","失败",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// set window topmost
        /// </summary>
        private void AddList_Load(object sender, EventArgs e)
        {
            TopMost = true;
        }
    }
}
