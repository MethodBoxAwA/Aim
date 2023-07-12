using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// show tools for test
    /// </summary>
    public partial class TestTools : MetroForm
    {
        /// <summary>
        /// absolutely useless,but i cannot remove it now :D
        /// </summary>
        Action<int> SignalCallback;

        /// <summary>
        /// initialize component and do some horrible things
        /// </summary>
        /// <param name="SignalCallback">i think it is terrible</param>
        public TestTools(Action<int> SignalCallback)
        {
            InitializeComponent();
            this.SignalCallback = SignalCallback;
        }

        private void TestTools_Load(object sender, EventArgs e)
        {
            listView_Main.LargeImageList = imageList_M;
            imageList_M.ImageSize = new Size(37, 36);
            imageList_M.Images.Add(Image.FromFile($"{Application.StartupPath}\\icon\\t_variable.png"));
            imageList_M.Images.Add(Image.FromFile($"{Application.StartupPath}\\icon\\t_errorcenter.png"));
            imageList_M.Images.Add(Image.FromFile($"{Application.StartupPath}\\icon\\t_signqueue.png"));
            listView_Main.Items.Add("变量查看").ImageIndex = 0;
            listView_Main.Items.Add("错误中心").ImageIndex = 1;
            listView_Main.Items.Add("信号队列").ImageIndex = 2;
        }

        /// <summary>
        /// show what user selected
        /// </summary>
        private void listView_Main_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            MessageBox.Show(listView_Main.SelectedItems[0].SubItems[0].Text);
        }

        /// <summary>
        /// open specific tool
        /// </summary>
        private void listView_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView_Main.SelectedItems[0].SubItems[0].Text
                    == "变量查看")
                {
                    new Testify().Show();
                }
                else if (listView_Main.SelectedItems[0].SubItems[0].Text 
                    == "错误中心")
                {
                    new ErrorCenter().Show();
                }
                else if (listView_Main.SelectedItems[0].SubItems[0].Text 
                    == "信号队列")
                {
                    MessageBox.Show("信号控制功能已从AimPlanner中被移除", 
                        "被移除的功能", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            // avoid cause any errors,if have,then hide them
            catch { }
        }
    }
}
