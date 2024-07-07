using System;
using MetroFramework.Forms;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace StonePlanner
{
    public partial class Register : MetroForm
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        int GW_CHILD = 5;
        const int EM_SETREADONLY = 0xcf;

        public Register()
        {
            InitializeComponent();
        }

        private void Register_Load(object sender, EventArgs e)
        {
            // Use password protection
            textBox_M_Pwd.UseSystemPasswordChar = true;

            // Set readonly
            IntPtr editHandle = GetWindow(comboBox_M_Type.Handle, GW_CHILD);
            SendMessage(editHandle, EM_SETREADONLY, 1, 0);
        }

        private void button_Submit_Click(object sender, EventArgs e)
        {
            // Confirm user had inputed corrsponding data
            if (textBox_M_Name.Text.Length == 0 || textBox_M_Pwd.Text.Length == 0)
            {
                MessageBox.Show("请输入用户名或密码！","注册失败",
                    MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                return;
            }
            Regex regex = new Regex("^[\u4E00-\u9FA5A-Za-z0-9]+$");
            bool isInvalid = (!regex.Match(textBox_M_Name.Text).Success) && 
                (!regex.Match(textBox_M_Pwd.Text).Success);
            
            // Confirm that the username and password is vaild
            if (isInvalid)
            {
                MessageBox.Show("您输入的用户名或密码含有特殊字符", "注册失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Create a vaild data
            if (comboBox_M_Type.Text != "Administrator" && 
                comboBox_M_Type.Text != "Standard")
            {
                comboBox_M_Type.Text = "Standard";
            }

            int accountType = comboBox_M_Type.Text == "Administrator" ? 0 : 1;
         
            // Create a key use for account restore
            var charActers = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var Chararr = new char[16];
            var random = new Random();
            for (int j = 0; j < Chararr.Length; j++)
            {
                Chararr[j] = charActers[random.Next(charActers.Length)];
            }
            var restoreKey = new String(Chararr);

            // Confirm does not exist user that have the same name
            var entity = AccessEntity.GetAccessEntityInstance();
            var mappingTable = new NonMappingTable();

            if (entity.GetElement<DataType.Structs.User, NonMappingTable>
                (mappingTable, "tb_Users", "UserName", textBox_M_Name.Text, true,
                true ,new List<string> { "ID"}).Count != 0)
            {
                MessageBox.Show("已经存在具有相同名称的用户!", "注册失败",
                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
              
            // Insert into database
            var user = new DataType.Structs.User();
            user.UserName = textBox_M_Name.Text;
            user.UserPassword = textBox_M_Pwd.Text;
            user.Wisdom = user.Lasting = user.Explosive = user.UserMoney = 0;
            user.RestoreKey = restoreKey;
            entity.AddElement(user, "tb_Users");

            // Show restore key
            MessageBox.Show($"您的恢复密钥是:{restoreKey}，已经复制到剪切板，请妥善保管。", "注册成功", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Clipboard.SetText(restoreKey);
            Close();
        }
    }
}