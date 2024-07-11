using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using static StonePlanner.Interfaces;
using static StonePlanner.Manager;

namespace StonePlanner
{
    public partial class Login : MetroForm
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;  
            textBox_M_Pwd.UseSystemPasswordChar = true;
        }

        private void button_Submit_Click(object sender, EventArgs e)
        {
            // Connect databse
            var entity = AccessEntity.GetAccessEntityInstance();
            var userInstance = entity.GetElement<DataType.Structs.User,IMappingTable>
                (new NonMappingTable(), "tb_Users","UserName", textBox_M_Name.Text, true,
                true, new List<string> { "ID" });

            // User is not exists
            if (userInstance.Count == 0)
            {
                MessageBox.Show("账号不存在！", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Wrong password
            if (userInstance[0].UserPassword != textBox_M_Pwd.Text) 
            {
                MessageBox.Show("用户名或密码错误!", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create account manager
            MessageBox.Show("登录成功", "登录成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            AccountManager accountManager = AccountManager.GetManagerInstance(textBox_M_Name.Text, userInstance[0].UserType.ToString());
            Main mainWindow = new Main();
            mainWindow.Show();
            Hide();
        }

        private void linkLabel_GetPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("准备进入找回密码流程，请准备：\n1、您的用户名；\n2、您的用户密钥；\n3、证明材料（可选择）。","找回密码"
                ,MessageBoxButtons.OK,MessageBoxIcon.Information);
            MessageBox.Show("还未开发完毕！", "找回密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Open register form
        /// </summary>
        private void linkLabel_Register_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new Register().Show();
        }
    }
}