﻿using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static StonePlanner.Develop.Sign;
using static StonePlanner.Exceptions;
using static StonePlanner.DataType.Structs;
using static StonePlanner.Interfaces;

/*
 * **************************************************************************
 * ********************                                  ********************
 * ********************      COPYRIGHT MethodBox 2024       *****************
 * ********************                                  ********************
 * **************************************************************************
 *                                                                          *
 *                                   _oo8oo_                                *
 *                                  o8888888o                               *
 *                                  88" . "88                               *
 *                                  (| -_- |)                               *
 *                                  0\  =  /0                               *
 *                                ___/'==='\___                             *
 *                              .' \\|     |// '.                           *
 *                             / \\|||  :  |||// \                          *
 *                            / _||||| -:- |||||_ \                         *
 *                           |   | \\\  -  /// |   |                        *
 *                           | \_|  ''\---/''  |_/ |                        *
 *                           \  .-\__  '-'  __/-.  /                        *
 *                         ___'. .'  /--.--\  '. .'___                      *
 *                      ."" '<  '.___\_<|>_/___.'  >' "".                   *
 *                     | | :  `- \`.:`\ _ /`:.`/ -`  : | |                  *
 *                     \  \ `-.   \_ __\ /__ _/   .-` /  /                  *
 *                 =====`-.____`.___ \_____/ ___.`____.-`=====              *
 *                                   `=---=`                                *
 * **************************************************************************
 * ********************                                  ********************
 * ********************      				             ********************
 * ********************         佛祖保佑 永无BUG           ********************
 * ********************                                  ********************
 * **************************************************************************
 */

namespace StonePlanner
{
    public partial class Main : Form
    {
        #region 主字段
        //常量
        const int DC_PLANHEIGHT = 36;
        //信号
        internal Signal signal = new Signal();
        //传出请求删除的请求体对象本身
        internal static Plan plan = null;
        internal static List<string> sentence = new List<string>();
        internal static List<string> nownn = new List<string>();
        //废弃任务数组
        public static List<Plan> recycle_bin = new List<Plan>();
        //TO-DO
        internal static UserPlan planner; //It is a void*
        //密码
        internal static string password = "methodbox5";
        //检查语言包
        internal static bool activation = true;
        internal static bool banned = false;
        //全局展示
        TaskDetails td;
        //数据库查询
        internal static OleDbConnection odcConnection = new OleDbConnection();
        #endregion

        #region 外部引用
        /// <summary>
        /// 该函数用来发送Windows消息（WM）处理窗口拖动事件。
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// 该函数从当前线程中的窗口释放鼠标捕获，并恢复通常的鼠标输入处理。
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        /// <summary>
        /// 该函数将拖动事件发送，以用于拖动窗口。
        /// </summary>
        private void panel_Top_MouseDown(object sender, MouseEventArgs e)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 0x0002;
            if (e.Button == MouseButtons.Left)  // 按下的是鼠标左键   
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr) HTCAPTION, IntPtr.Zero);// 拖动窗体  
            }
        }
        #endregion


        #region 主窗口及设置加载/退出
        public Main()
        {
            InitializeComponent();
            Settings settings = new Settings();
            settings.Dispose();

            signal.SignChanged += HandleSign;
        }

        internal void HandleSign(object sender, DataType.SignChangedEventArgs e)
        {
            if (e.Sign == 1)
            {
                //链接数据库
                string strConn = $" Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = {Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password}";
                OleDbConnection myConn = new OleDbConnection(strConn);
                myConn.Open();
                //先搜一下数据库
                var hResult = SQLConnect.SQLCommandQuery($"SELECT * FROM Tasks WHERE ID = {plan.ID}");
                if (hResult.HasRows)
                {
                    string updateString = $"UPDATE Tasks SET TaskTime = {plan.Seconds}" +
                                $" , TaskStatus = \"已办完\"" +
                                $" WHERE ID = {plan.ID}";
                    SQLConnect.SQLCommandQuery(updateString, ref Main.odcConnection);
                }
                else
                {
                    string strInsert = " INSERT INTO Tasks ( TaskName , TaskIntro , TaskStatus , " +
                        "TaskTime , TaskDiff ,TaskLasting ,TaskExplosive , TaskWisdom , ID" +
                        " , TaskParent) VALUES ( ";
                    strInsert += "'" + plan.Capital + "', '";
                    strInsert += plan.Intro + "', '";
                    strInsert += plan.Status + "', ";
                    strInsert += plan.Seconds + ", ";
                    strInsert += plan.Difficulty + ",";
                    strInsert += plan.Lasting + ",";
                    strInsert += plan.Explosive + ",";
                    strInsert += plan.Width + ",";
                    strInsert += plan.ID + ",";
                    strInsert += "'" + plan.Parent + "'" + ")";
                    //执行插入
                    OleDbCommand inst = new OleDbCommand(strInsert, myConn);
                    inst.ExecuteNonQuery();
                }

                //删除
                var manager = Manager.SerialManager.GetManagerInstance();
                recycle_bin.Add(plan);
                manager.RemoveTask(plan.Serial);
                plan = null;
                LengthCalculation();

            }

            //已废弃：Sign == 1，添加任务
            if (e.Sign == 2)
            {
                if (panel_L.Width <= 120)
                {
                    panel_L.Width += 2;
                    AddSignal(2);
                }
            }

            else if (e.Sign == 3)
            {
                if (panel_L.Width > 0)
                {
                    panel_L.Width -= 2;
                    AddSignal(3);
                }
            }

            //已废弃：Sign == 4，添加任务
            else if (e.Sign == 5)
            {
                ExportTodo et = new ExportTodo(panel_M.Controls);
                et.Show();
            }

            else if (e.Sign == 6)
            {
                panel_TaskDetail.Controls.Remove(td);
                if (plan == null)
                    return;
                td = new TaskDetails((Action<int>) AddSignal);
                td.Left = 16;
                td.Top = 15;
                td.Capital = plan.Capital;
                td.Time = plan.Seconds.ToString();
                td.Intro = plan.Intro;
                td.StatusResult = plan.Status;
                td.Difficulty = plan.Difficulty;
                td.Lasting = plan.Lasting.ToString();
                td.Explosive = plan.Explosive.ToString();
                td.Wisdom = plan.Wisdom.ToString();
                SoundPlayer sp = new SoundPlayer($@"{Application.StartupPath}\icon\Click.wav");
                sp.Play();
                panel_TaskDetail.Controls.Add(td);
                td.BringToFront();
            }
            else if (e.Sign == 7)
            {
                panel_TaskDetail.Controls.Remove(td);
                SoundPlayer sp = new SoundPlayer($@"{Application.StartupPath}\icon\Click.wav");
                sp.Play();
            }
            //不存在：Sign == 8
            else if (e.Sign == 9)
            {
                pictureBox_Tip.Visible = false;
            }
            else if (e.Sign == 10)
            {
                //怀疑出现的问题：重复执行
                GetSchedule();
            }
            //不存在：Sign == 11
            else if (e.Sign == 12)
            {
                if (Width >= 256)
                {
                    Width -= 2;
                    pictureBox_T_Exit.Left -= 2;
                    AddSignal(12);
                }
            }
            else if (e.Sign == 13)
            {
                if (Width <= 674)
                {
                    Width += 2;
                    pictureBox_T_Exit.Left += 2;
                    AddSignal(13);
                }
            }
        }

        /// <summary>
        /// 覆写窗体的消息处理函数
        /// </summary>
        /// <param name="m">消息</param>
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case AM_EXIT:
                    Environment.Exit(0);
                    break;
                case AM_ADDMONEY:
                    var moneyManager = Manager.MoneyManager.GetManagerInstance();
                    moneyManager.Change(m.WParam.ToInt32());
                    break;
                case AM_GETMONEY:
                    moneyManager = Manager.MoneyManager.GetManagerInstance();
                    SendMessage(m.WParam, AM_GETMONEY, (IntPtr) moneyManager.GetValue(), IntPtr.Zero);
                    break;
                //调用基类函数，以便系统处理其它消息。
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// 该函数处理用户退出事件，存入新的还未存储的数据。
        /// </summary>
        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            var manager = Manager.SerialManager.GetManagerInstance();
            //存入还未完成的任务
            foreach (var plan in manager.GetList())
            {
                //先判断是否存在
                //Users可还行 表都他妈不分了吗你
                if (plan != null)
                {
                    /*
                     * 此处的Bug：
                     * 关闭的时候，检查是否已经存在了相应任务
                     * 如果存在就不在添加
                     * 但是，如果已经更新了数据
                     * 也无法存储，导致了任务还原的Bug
                     * 做法：
                     * 仅对状态为待办的剩余时间和是否完成进行更新
                    */
                    string queryString = $"SELECT * FROM Tasks WHERE ID = {plan.ID}";
                    var sqlResult = SQLConnect.SQLCommandQuery(queryString, ref Main.odcConnection);
                    if (sqlResult.HasRows)
                    {
                        //已经存在相应任务，查询是否已完成，否则更新时间
                        sqlResult.Read();
                        //查询状态 不为待办
                        if (sqlResult["TaskStatus"].ToString() != "待办")
                        {
                            //MessageBox.Show(sqlResult["TaskStatus"].ToString());
                            continue;
                        }
                        else
                        {
                            //更新时间和待办状态
                            //UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                            if (plan.Seconds > 0)
                            {
                                string updateString = $"UPDATE Tasks SET TaskTime = {plan.Seconds}" +
                                    $" WHERE ID = {plan.ID}";
                                SQLConnect.SQLCommandQuery(updateString, ref Main.odcConnection);
                                continue;
                            }
                            else
                            {
                                ErrorCenter.AddError(DataType.ExceptionsLevel.Warning
                                    , new ObjectFreedException("已经被清除的任务再次添加。"));
                            }
                        }
                    }
                    //脑子是个好东西 下次带上
                    string strInsert = "INSERT INTO Tasks ( TaskName , TaskIntro , TaskStatus , TaskTime , TaskDiff ,TaskLasting ,TaskExplosive , TaskWisdom , TaskParent , StartTime) VALUES ( ";
                    strInsert += "'" + plan.Capital + "', '";
                    strInsert += plan.Intro + "', '";
                    strInsert += plan.Status + "', ";
                    strInsert += plan.Seconds + ", ";
                    strInsert += plan.Difficulty + ",";
                    strInsert += plan.Lasting + ",";
                    strInsert += plan.Explosive + ",";
                    strInsert += plan.Wisdom + ",";
                    strInsert += "'" + plan.Parent + "',";
                    strInsert += "'" + plan.StartTime + "')";
                    //执行插入
                    SQLConnect.SQLCommandExecution(strInsert, ref Main.odcConnection);
                    recycle_bin.Add(plan);
                }
            }
            //关闭数据库连接
            Main.odcConnection.Close();
            Environment.Exit(0);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            #region 窗口加载
            this.TopMost = false;

            Recycle recy_bin = new Recycle();
            GC.Collect();

            var accountManager =
                Manager.AccountManager.GetManagerInstance();

            // Connect database & constuct user instance
            var entity = AccessEntity.GetAccessEntityInstance();
            var users = entity.GetElement<User, IMappingTable>(
                new NonMappingTable(), "tb_Users", "UserName", accountManager.GetValue().Item1, 
                 true, true, new List<string>() { "ID" });
            var userInstance = users[0];

            // Create money manager for global
            var moneyManager =
                Manager.MoneyManager.GetManagerInstance(userInstance.UserMoney);

            // Create property manager for global
            var propertyManager =
                Manager.PropertyManager.GetManagerInstance(
                    userInstance.Lasting,
                    userInstance.Explosive,
                    userInstance.Wisdom);

            label_GGS.Text = userInstance.UserMoney.ToString();
            Thread valueThread = new Thread(new ThreadStart(ValueGetter));
            valueThread.Start();
            //pictureBox_Main.ImageLocation = "https://tse1-mm.cn.bing.net/th/id/R-C.2fd0dadf9d13c716cf0494d17875cf3b?rik=mf3ZQjupoBDr2A&riu=http%3a%2f%2fup.36992.com%2fpic%2f07%2fd3%2fe8%2f07d3e81f37f5922b5b0021a1c0b2d3da.jpg&ehk=P8hpii3cUJykmCt97WX0kATyROzUNRuexj8faXE7q6c%3d&risl=&pid=ImgRaw&r=0";
            //获取格言
            Thread sentenceGetter = new Thread(() => SentenceGetter());
            sentenceGetter.Start();
            label_Date.Text = DateTime.Now.ToString("dd");
            label_Month.Text = DateTime.Now.ToString("MM");
            #endregion
            CheckForIllegalCrossThreadCalls = false;
            #region 未完成任务读取
            for (int i = 0; i < recy_bin.dataGridView1.Rows.Count - 1; i++)
            {
                if (recy_bin.dataGridView1.Rows[i].Cells[5].Value.ToString() == "0")
                {
                    continue;
                }

                UserPlan userPlan = new()
                {
                    Capital = recy_bin.dataGridView1.Rows[i].Cells[1].Value.ToString(),
                    Intro = recy_bin.dataGridView1.Rows[i].Cells[2].Value.ToString(),
                    Seconds = Convert.ToInt32(recy_bin.dataGridView1.Rows[i].Cells[5].Value),
                    Difficulty = Convert.ToDouble(recy_bin.dataGridView1.Rows[i].Cells[4].Value),
                    ID = Convert.ToInt32(recy_bin.dataGridView1.Rows[i].Cells[0].Value),
                    Lasting = Convert.ToInt32(recy_bin.dataGridView1.Rows[i].Cells[6].Value),
                    Explosive = Convert.ToInt32(recy_bin.dataGridView1.Rows[i].Cells[7].Value),
                    Wisdom = Convert.ToInt32(recy_bin.dataGridView1.Rows[i].Cells[8].Value),
                    StartTime = Convert.ToString(recy_bin.dataGridView1.Rows[i].Cells[10].Value),
                    AddSign = (Action<int>) AddSignal,
                    BuildMode = PlanBuildMode.B
                };

                AddPlan(new Plan(userPlan));
                LengthCalculation();
                plan = null;
            }
            #endregion
            sentence.FindAll(sentence => sentence.Contains("\n")).ForEach(sentence => sentence.Replace("\n", ""));
            #region 功能控制器
            if (!activation)
            {
                label_Sentence.Text = "MethodBox Aim（评估副本）";
            }
            if (banned)
            {
                label_Sentence.Text = "MethodBox Aim（限制副本）";
            }
            #endregion
            string alert = GetSchedule(true);
            ScanTaskTime(alert);
            contextMenuStrip.Enabled = false;
        }
        #endregion
        delegate void PlanAddInvoke(Plan pValue);
        #region 任务处理相关
        internal void AddPlan(Plan task)
        {
            //分配唯一编号
            var manager = Manager.SerialManager.GetManagerInstance();
            task.Top = manager.AddTask(task);
            panel_M.Controls.Add(task);
            LengthCalculation();
        }

        protected void ScanTaskTime(string alert = "")
        {
            List<string> _tasks = new List<string>();
            foreach (var item in panel_M.Controls)
            {
                Plan temp;
                if (item is not Plan)
                {
                    continue;
                }
                else
                {
                    temp = item as Plan;
                }
                if (temp.Status != "正在办")
                {
                    if (DateTime.Now.ToBinary() >= temp.StartTime)
                    {
                        _tasks.Add(temp.Capital);
                    }
                }
            }
            if (_tasks.Count != 0)
            {
                new Alert(_tasks, alert).ShowDialog();
            }
            _tasks.Clear();
        }


        #endregion
        #region 加载器
        //列表加载器
        protected void HoldList()
        {
            //MYUKKE IS GOOOOOOD!
            //像暂时存储副控件添加新的Controls.Add函数
            panel_L.ControlAdded += new ControlEventHandler(Another_OnControlAdded);
            panel_L.Controls.Clear();
            //这个字段是用来连接用的
            //string strConn = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\data.mdb;Jet OLEDB:Database Password={Main.password}";
            //OleDbConnection odcConnection = new OleDbConnection(strConn); //打开连接
            //odcConnection.Open(); //建立SQL查询   
            //OleDbCommand odCommand = odcConnection.CreateCommand();
            //搜索数据库中所有列表
            List<string> list = new List<string>();
            //OleDbConnection odcConnectiontemp = new OleDbConnection();
            //var sResult = SQLConnect.SQLCommandQuery("SELECT * FROM Lists",ref odcConnectiontemp);
            //odCommand.CommandText = "SELECT * FROM Lists";
            var sResult = SQLConnect.SQLCommandQuery("SELECT * FROM Lists", ref Main.odcConnection);
            while (sResult.Read())
            {
                list.Add(sResult[1].ToString());
            }
            //对所有列表 依次搜索其子值
            //这里读取次数太多，就不用封装好的查询了
            //引用关闭原有数据库连接
            List<Plan> sonTask = new List<Plan>();
            //猜猜DataReader在哪儿，小子
            foreach (var item in list)
            {
                //添加父节点
                Function parentMain = new Function(item, item, 1);
                panel_L.Controls.Add(parentMain);
                /*
                 * '已有打开的与此 Command 相关联的 DataReader，必须首先将它关闭。'
                 * 他妈的 DataReader在哪儿
                 * 谁来告诉我
                 * 淦我自己觉得一点问题没有
                 * 里边关里边不行 用了引用外边关
                 * 还是不行 怎么也不行
                 * 百度说用一个连接参数 我直接连接报错
                 * 奶奶的 真是绝了
                 *
                 * 怀疑：读空报错
                 *
                 * 果然是读空报错
                 * 怀疑原因是自动跳出之后再次尝试读取
                 * 你的报错能不能走点心啊
                 */
                try
                {
                    sResult = SQLConnect.SQLCommandQuery($"SELECT * FROM Tasks WHERE TaskParent = '{item}'");
                }
                catch { return; }
                //建立Plan对象
                //1 5 2 4 9 6 7 8
                while (sResult.Read())
                {
                    UserPlan userPlan = new()
                    {
                        Capital = sResult[1].ToString(),
                        Intro = sResult[2].ToString(),
                        Seconds = Convert.ToInt32(sResult[5]),
                        Difficulty = Convert.ToInt64(sResult[4]),
                        ID = Convert.ToInt32(sResult[0]),
                        Lasting = Convert.ToInt32(sResult[6]),
                        Explosive = Convert.ToInt32(sResult[7]),
                        Wisdom = Convert.ToInt32(sResult[8]),
                        AddSign = (Action<int>) AddSignal,
                        BuildMode = PlanBuildMode.B
                    };

                    using Plan plan = new Plan
                    (
                          userPlan
                    );
                    Function sonMain = new Function(sResult[1].ToString(), item, 0); panel_L.Controls.Add(sonMain);
                }
            }
            panel_L.ControlAdded -= Another_OnControlAdded;
            return;
        }

        protected void LoadFunction()
        {
            if (this.InvokeRequired)
                this.Invoke(LoadFunction);
            else
            {
                // Functions
                AddTodo.PlanAddInvoke officalInvoke = new AddTodo.PlanAddInvoke(AddPlan);
                Function newTodo = new Function($"{Application.StartupPath}\\icon\\new.png",
                    $"新建任务", "AddToDo", officalInvoke, (Action<int>) AddSignal)
                {
                    Top = 0
                };
                panel_L.Controls.Add(newTodo);

                LoadSignalFunction("任务回收", "Recycle", "recycle", 1);
                LoadSignalFunction("我的商城", "Shop", "shop", 2);
                LoadSignalFunction("主控制台", "Console", "console", 3);
                LoadSignalFunction("事件编写", "InnerIDE", "program", 4);
                LoadSignalFunction("在线协作", "WebService", "server", 5);
                LoadSignalFunction("软件升级", "Update", "update", 6);
                LoadSignalFunction("调试工具", "Debugger", "debug", 7);
                LoadSignalFunction("关于软件", "About", "info", 9);

                Function Schedule = new($"{Application.StartupPath}\\icon\\schedule.png",
                    $"排班日历", "Schedule", (Action<int>) AddSignal)
                {
                    Top = 8 * 34
                };
                panel_L.Controls.Add(Schedule);

                //Bottom
                Bottom Function = new("功能")
                {
                    Top = 374,
                    Left = 1
                };
                Function.Click += label_L_Function_Click;
                Function.label_B.Click += label_L_Function_Click;
                panel_L.Controls.Add(Function);
                Bottom Type = new("分类")
                {
                    Top = 374,
                    Left = 60
                };
                Type.Click += label_L_Type_Click;
                Type.label_B.Click += label_L_Type_Click;
                panel_L.Controls.Add(Type);
                label_Status.Text = "正在休息";
            }
            return;
        }

        public void LoadSignalFunction(string name, string prompt, string icon, int i)
        {
            Function function = new($"{Application.StartupPath}\\icon\\{icon}.png",
                   name, prompt)
            {
                Top = 34 * i
            };
            panel_L.Controls.Add(function);
        }

        #endregion

        private void timer_Anti_Tick(object sender, EventArgs e)
        {
            // Get parent process infomation
            if (Process.GetCurrentProcess().Parent().ProcessName != "explorer.exe" && Process.GetCurrentProcess().Parent().ProcessName != "msvsmon")
            {
                var entity = AccessEntity.GetAccessEntityInstance();
                string processName;

                try
                {
                    processName = Process.GetCurrentProcess().Parent().ProcessName.Split('.')[0];
                }
                catch
                {
                    processName = Process.GetCurrentProcess().Parent().ProcessName;
                }

                // Confirm have not any error
                if (processName == "explorer") return;

                // Ban account
                var banUser = new User();
                banUser.UserName = "METHODBOX_BAN";
                entity.AddElement(banUser, "tb_Users", new List<string> { "ID" });

                // Terminate process
                Environment.Exit(-114);
            }

            nownn.Clear();
            string[] files;
            foreach (Control item in panel_M.Controls)
            {
                if (item.GetType() == typeof(Plan))
                {
                    nownn.Add((item as Plan).Capital);
                }
            }

            try
            {
                files = Directory.GetFiles(Application.StartupPath, "*.dll");

                if (files.Length != 0)
                {
                    BanUser();
                }
            }
            catch
            {
                BanUser();
            }
            
            try
            {
                var result = SQLConnect.SQLCommandQuery($"SELECT * FROM Users where Username='METHODBOX_BAN';");
                result.Read();
                if (result[0].ToString() != "" || result[0].ToString() != null)
                {
                    Ban ban = new Ban();
                    Opacity = 0;
                    int isCritical = 1;
                    int BreakOnTermination = 0x1D;
                    Process.EnterDebugMode();  //acquire Debug Privileges
                                               // setting the BreakOnTermination = 1 for the current process
                    NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
                    //for (int i = 0; ; i++) { System.Console.WriteLine(i); }
                }
            }
            catch { }
        }

        #region 金钱操作
        public void ValueGetter()
        {
            var moneyManager = Manager.MoneyManager.GetManagerInstance();
            while (true)
            {
                label_GGS.Text = moneyManager.GetValue().ToString();
                Thread.Sleep(1000);
            }
        }

        #endregion
        #region 加载排班日历
        internal string GetSchedule(bool @out = false)
        {
            Dictionary<DateTime, string> returns = new Dictionary<DateTime, string>();
            //内置
            foreach (var item in panel_M.Controls)
            {
                if (item is Plan)
                {
                    DateTime d = DateTime.FromBinary((item as Plan).StartTime);
                    if (returns.ContainsKey(d))
                    {
                        //频率统计
                        continue;
                    }
                    else
                    {
                        Plan plan1 = (item as Plan);
                        string sch = DateTime.FromBinary(plan1.StartTime).Hour switch
                        {
                            > 6 and < 15 => " 白班",
                            _ => " 夜班",
                        };
                        returns.Add(d, sch);
                    }
                }
            }
            try
            {
                if (@out)
                {
                    string tql = "";
                    new SchedulingCalendar(returns, out tql, @out).Show();
                    return tql;
                }
                else
                {
                    string _ = null;
                    new SchedulingCalendar(returns, out _, @out).Show();
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorCenter.AddError(DataType.ExceptionsLevel.Error, ex);
                return null;
            }
        }
        #endregion
        #region 信号系统
        internal void AddSignal(int sign)
        {
            signal.AddSignal(sign);
        }

        private void pictureBox_T_More_Click(object sender, EventArgs e)
        {
            if (panel_L.Controls.Count == 0)
            {
                Thread loaderThread = new Thread(new ThreadStart(LoadFunction));
                loaderThread.Start();
            }

            if (panel_L.Width == 0)
            {
                AddSignal(2);
            }
            else
            {
                AddSignal(3);
            }
        }
        #endregion
        #region 每日一句/一图加载器
        public void SentenceGetter()
        {
            try
            {
                WebClient MyWebClient = new WebClient
                {
                    Credentials = CredentialCache.DefaultCredentials//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                };
                Byte[] pageData = MyWebClient.DownloadData("http://methodbox.top/wkgd/Services/StonePlanner/sentence.txt"); //下载                                                                                            //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                foreach (var item in pageHtml.Split(';'))
                {
                    sentence.Add(item);
                }
            }
            catch (Exception ex)
            {
                ErrorCenter.AddError(DataType.ExceptionsLevel.Caution, ex);
                sentence.Add("浪费时间叫虚度，剥用时间叫生活。");
            }
            return;
        }
        #endregion
        #region 每日一句/一图执行器
        private void label_Sentence_TextChanged(object sender, EventArgs e)
        {
            label_Sentence.Text = label_Sentence.Text.Replace("\n", "");
        }
        #endregion
        #region 外观控制
        private void label_Sentence_MouseDown(object sender, MouseEventArgs e)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 0x0002;
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr) HTCAPTION, IntPtr.Zero);// 拖动窗体  
            }
        }

        private void User_Piicture_DoubleClick(object sender, EventArgs e)
        {
            ErrorCenter _ = new ErrorCenter();
            _.Show();
        }

        private void label_L_Function_Click(object sender, EventArgs e)
        {
            MouseWheel -= panel_L_MouseWheel;
            panel_L.Controls.Clear();
            LoadFunction();
        }

        private void label_L_Type_Click(object sender, EventArgs e)
        {
            //展示类列表
            //啥也不说了 绝了 真他妈绝了
            //添加鼠标滚动事件
            MouseWheel += panel_L_MouseWheel;
            panel_L.Controls.Clear();
            //将做好的数据加入
            HoldList();
        }

        /// <summary>
        /// 分类框鼠标中键滚动响应事件
        /// </summary>
        protected void panel_L_MouseWheel(object sender, MouseEventArgs e)
        {
            //在？不在？
            Rectangle pnlRightRectToForm1 = this.panel_L.ClientRectangle; // 获得Panel的矩形区域
            pnlRightRectToForm1.Offset(this.panel_L.Location);
            if (!pnlRightRectToForm1.Contains(e.Location)) return;
            if (e.Delta > 0) // 向上滚动
            {

                if (panel_L.Top >= 0)
                {
                    panel_L.Top = 0;
                    //恢复底部控件
                    Bottom Function = new Bottom("功能");
                    Function.Top = 374;
                    Function.Left = 1;
                    Function.Click += label_L_Function_Click;
                    Function.label_B.Click += label_L_Function_Click;
                    panel_L.Controls.Add(Function);
                    Bottom Type = new("分类")
                    {
                        Top = 374,
                        Left = 60
                    };
                    Type.Click += label_L_Type_Click;
                    Type.label_B.Click += label_L_Type_Click;
                    panel_L.Controls.Add(Type);
                    return;
                }
                //动态延长
                panel_L.Height += DC_PLANHEIGHT;
                panel_L.Top += DC_PLANHEIGHT;
            }
            else // 向下滚动
            {
                panel_L.Height += DC_PLANHEIGHT;
                panel_L.Top -= DC_PLANHEIGHT;
            }
            //移除底部按钮
            foreach (var item in panel_L.Controls.Find("buttom", true))
            {
                panel_L.Controls.Remove(item);
            }
            panel_L.BringToFront();
        }

        /// <summary>
        /// 动态计算框长度
        /// </summary>
        internal void LengthCalculation()
        {
            var manager = Manager.SerialManager.GetManagerInstance();
            int count = manager.TaskCount;
            if (count <= 10)
            {
                try
                {
                    vScrollBar_Main.Scroll -= Display;
                }
                catch { }
                return;
            }
            else
            {
                count -= 10;
                panel_M.Height = 397 + count * DC_PLANHEIGHT;
                vScrollBar_Main.Maximum = count * DC_PLANHEIGHT;
                vScrollBar_Main.Scroll += Display;
                return;
            }
        }

        private void Display(object sender, EventArgs e)
        {
            panel_M.SendToBack();
            panel_M.Top = 30 - vScrollBar_Main.Value;
            panel_L.Top = vScrollBar_Main.Value;
        }

        //覆写添加控件事件，使其按照顺序添加
        protected void Another_OnControlAdded(object sender, ControlEventArgs e)
        {
            //底下的菜单不被考虑在内
            if (e.Control.GetType() == typeof(Bottom))
                return;
            //获取已有控件 34高
            int i = panel_L.Controls.Count;
            e.Control.Top = (i - 1) * 34;
            //回调基类原函数 添加控件
            base.OnControlAdded(e);
        }
        #endregion
        #region 右键菜单事件
        private void 最大化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSignal(13);
        }

        private void 最小化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSignal(12);
        }

        private void 添加任务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddTodo _ = new AddTodo(AddPlan, (Action<int>) AddSignal);
            _.Show();
        }

        private void 任务回收站ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recycle _ = new Recycle();
            _.Show();
        }

        private void 新建清单ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddList _ = new AddList();
            _.Show();
        }

        private void 控制台ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console _ = new Console();
            _.Show();
        }

        private void 事件IDEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InnerIDE _ = new InnerIDE();
            _.Show();
        }

        private void 登录服务器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebService _ = new WebService();
            _.Show();
        }

        private void 静态指示器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Testify _ = new Testify();
            _.Show();
        }

        private void 信号控制器ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("信号控制功能已从AimPlanner中被移除", "被移除的功能", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void 错误中心ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ErrorCenter _ = new ErrorCenter();
            _.Show();
        }

        private void 引发崩溃ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new Exception("用户引发崩溃");
        }

        private void 访问主页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://space.bilibili.com/497309497?spm_id_from=333.1007.0.0");
        }

        private void 捐赠软件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://afdian.net/a/MethodBox");
        }
        private void 堵塞执行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSignal(114514);
        }

        private void 恢复执行ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSignal(2);
        }

        private void 停用商城ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Shop _ = new Shop();
            _.Show();
        }

        private void 停用导入包ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Importer _ = new Importer();
            _.Show();
        }

        private void 停用导出待办ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportTodo _ = new ExportTodo(panel_M.Controls);
            _.Show();
        }
        #endregion

        private void BanUser() 
        {
            // Ban account
            var banUser = new User();
            banUser.UserName = "METHODBOX_BAN";
            var entity = AccessEntity.GetAccessEntityInstance();
            entity.AddElement(banUser, "tb_Users", new List<string> { "ID" });

            // Terminate process
            Environment.Exit(-114);
        }

        private void pictureBox_T_Float_DoubleClick(object sender, EventArgs e)
        {
            contextMenuStrip.Enabled = !contextMenuStrip.Enabled;
        }

        private void User_Piicture_Click(object sender, EventArgs e)
        {
            UserInfo info = new UserInfo();
            info.Show();
        }

        private void pictureBox_T_Float_Click(object sender, EventArgs e)
        {
            Update update = new Update();
            update.Show();
        }
    }

}