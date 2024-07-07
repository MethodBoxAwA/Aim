using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MetroFramework.Forms;
using static StonePlanner.Manager;

namespace StonePlanner
{
    public partial class WebService : MetroForm
    {
        protected static Socket cilent;
        public WebService()
        {
            InitializeComponent();
        }

        private void linkLabel_Register_LinkClicked(object sender, EventArgs e)
        {
            MessageBox.Show("请联系您的网络管理员操作", "提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_Submit_Click(object sender, EventArgs e)
        {
            cilent = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint iPEndPoint = null;
            try
            {
                string ip = textBox_M_Address.Text;
                int port = Convert.ToInt32(textBox_M_Port.Text);
                IPAddress iPAddress = IPAddress.Parse(ip);
                iPEndPoint = new IPEndPoint(iPAddress, port);
            }
            catch
            {
                MessageBox.Show("信息输入有误，请重试", "连接创建失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                cilent.Connect(iPEndPoint);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"连接失败，可能是远程主机已关闭。\n具体原因是：{ex.Message}。", "连接失败"
                    , MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            //尝试登录
            string command = $"-login {textBox_M_Name.Text} {textBox_M_Pwd.Text}";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(command);
            cilent.Send(buffer);

            Thread receiveThread = new Thread(Receive);
            receiveThread.IsBackground = true;
            receiveThread.Start(cilent);
        }

        protected void Receive(object socket)
        {
            Socket receiver = socket as Socket;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    receiver.Receive(buffer, SocketFlags.None);
                    string serverInfo = Encoding.UTF8.GetString(buffer);
                    try
                    {
                        var dResult = (Dictionary<string, string>) ByteConvert.BytesToObject(buffer);
                        if (dResult.Count != 0)
                        {
                            foreach (var item in dResult)
                            {
                                MessageBox.Show($"{item.Value}：{item.Key}");
                            }
                        }
                        //WebUser ws = new WebUser(reveiver);
                        //ws.Show();
                        Hide();
                        //登陆成功，隐藏自身。
                    }
                    catch { }
                    try
                    {
                        var dResult = (DataType.Structs.PlanStruct) ByteConvert.BytesToObject(buffer);
                        AddTodo addtodo = new AddTodo(dResult);
                        MethodInvoker fmShower = new MethodInvoker(() => addtodo.Show());
                        BeginInvoke(fmShower);
                    }
                    catch { }
                    if (serverInfo.Replace("\0", "") == "-Getinfo")
                    {
                        DataType.Structs.UserStruct ubuff = new DataType.Structs.UserStruct(); 
                        var moneyManager = Manager.MoneyManager.GetManagerInstance();
                        var propertyManager = Manager.PropertyManager.GetManagerInstance();
                        var accountManager = Manager.AccountManager.GetManagerInstance();

                        ubuff.userName = accountManager.GetValue().Item1;

                        ubuff.userMoney = moneyManager.GetValue();
                        ubuff.userExplosive = propertyManager.Explosive;
                        ubuff.userWisdom = propertyManager.Wisdom;
                        ubuff.userLasting = propertyManager.Lasting;
                        byte[] buffer_send = ByteConvert.ObjectToBytes(ubuff);
                        receiver.Send(buffer_send, SocketFlags.None);
                    }
                    if (serverInfo.Replace("\0", "") == "-Gettask")
                    {
                        Random rd = new Random();
                        Dictionary<string, int> taskTypePairs = new Dictionary<string, int>();
                        foreach (var item in Main.recycle_bin)
                        {
                            try
                            {
                                taskTypePairs.Add(item.Capital, 1);
                            }
                            catch
                            {
                                taskTypePairs.Add(item.Capital + rd.Next(0, 10000), 1);

                            }
                        }
                        foreach (var item in Main.nownn)
                        {
                            try
                            {
                                taskTypePairs.Add(item, 2);
                            }
                            catch
                            {
                                taskTypePairs.Add(item + rd.Next(0, 10000), 1);

                            }
                        }
                        byte[] buffer_send = ByteConvert.ObjectToBytes(taskTypePairs);
                        receiver.Send(buffer_send, SocketFlags.None);
                    }
                    //MessageBox.Show($"服务器信息：{System.Text.Encoding.UTF8.GetString(buffer)}");
                }
                catch { (socket as Socket).Close(); return; }
            }
        }

        private void WebService_Load(object sender, EventArgs e)
        {

        }
    }
}
