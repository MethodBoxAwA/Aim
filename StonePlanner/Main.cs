using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static StonePlanner.DataType.Structs;
using static StonePlanner.Exceptions;
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
        private readonly SynchronizationContext UISyncContext;
        //全局展示
        TaskDetails td;
        #endregion

        #region 外部引用[F]
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
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
            // Catch current synchronization context
            UISyncContext = SynchronizationContext.Current;

            // Load window
            InitializeComponent();
            Settings settings = new Settings();
            settings.Dispose();

            // Construct sign handler
            signal.SignChanged += HandleSign;
        }

        internal void ShowDetails(Plan task)
        {
            // Delete existed control
            panel_TaskDetail.Controls.Remove(td);
            if (task is null)
                return;

            // Build new details panel
            td = new TaskDetails((Action<int>) AddSignal);
            td.Left = 16;
            td.Top = 15;
            td.Capital = task.Capital;
            td.Time = task.Seconds.ToString();
            td.Intro = task.Intro;
            td.Difficulty = task.Difficulty;
            td.Lasting = task.Lasting.ToString();
            td.Explosive = task.Explosive.ToString();
            td.Wisdom = task.Wisdom.ToString();

            // Play sound to simulate click
            SoundPlayer sp = new SoundPlayer($@"{Application.StartupPath}\icon\Click.wav");
            sp.Play();
            panel_TaskDetail.Controls.Add(td);

            // Move to top
            td.BringToFront();
        }

        internal void HandleSign(object sender, DataType.SignChangedEventArgs e)
        {
            // Task finished
            if (e.Sign == 1)
            {
                // Build eneity
                var entity = AccessEntity.GetAccessEntityInstance();
                plan.Status = "已办完";
                var task = entity.GetElement<UserPlan, NonMappingTable>(
                    new NonMappingTable(), "tb_Tasks", "ID", plan.ID.ToString());

                // Exists current plan
                if (task.Count != 0)
                {
                    entity.UpdateElement(new UserPlan(plan), new NonMappingTable(), "ID",
                        "tb_Tasks", new List<string> { "ID", "BuildType" });
                }
                // Not
                else
                {
                    entity.AddElement(new UserPlan(plan), "tb_Tasks",
                        new List<string> { "ID", "BuildType" });
                }

                // Delete from software
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

            // [Obsolete]Sign == 4, Add Task
            else if (e.Sign == 5)
            {
                ExportTodo et = new ExportTodo(panel_M.Controls);
                et.Show();
            }

            // [Obsolete]Sign == 6, Show details
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

        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            // Get task serial manager
            var manager = Manager.SerialManager.GetManagerInstance();

            // Iterate remained plan list
            foreach (var plan in manager.GetList())
            {
                if (plan != null)
                {
                    // Build instance
                    var entity = AccessEntity.GetAccessEntityInstance();
                    var planFromDB = entity.GetElement<UserPlan, NonMappingTable>(
                        new NonMappingTable(), "tb_Tasks", "ID", plan.ID.ToString());

                    // Plan is exists
                    if (planFromDB.Count != 0)
                    {
                        // Exists,switch state
                        if (planFromDB[0].Status != "待办")
                        {
                            continue;
                        }

                        // Update plan
                        if (plan.Seconds > 0)
                        {
                            var userPlan = new UserPlan(plan);
                            entity.UpdateElement(userPlan, new NonMappingTable(), "ID", "tb_Tasks", new List<string> { "ID", "BuildMode" });
                            continue;
                        }

                        else
                        {
                            ErrorCenter.AddError(DataType.ExceptionsLevel.Warning
                                , new ObjectFreedException("已经被清除的任务再次添加。"));
                            continue;
                        }
                    }

                    // Plan is not exists
                    entity.AddElement(new UserPlan(plan), "tb_Tasks", new List<string> { "ID", "BuildMode" });
                }
            }

            Environment.Exit(0);
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            // Set low postion
            this.TopMost = false;

            // Prepare account info
            var accountManager =
                Manager.AccountManager.GetManagerInstance();

            // Connect database & constuct user instance
            var entity = AccessEntity.GetAccessEntityInstance();
            var users = entity.GetElement<User, IMappingTable>(
                new NonMappingTable(), "tb_Users", "UserName", accountManager.GetValue().Item1,
                 true, true);
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

            label_Money.Text = userInstance.UserMoney.ToString();

            // Display money
            moneyManager.MoneyChanged += (money) => label_Money.Text = money.ToString();

            // Get sentences
            var sentences = await GetSentence();

            // Set sentences
            // Here comes a cool effect even although it is a bug
            sentences.FindAll(sentence => sentence.Contains("\n")).ForEach(sentence => sentence.Replace("\n", ""));
            ThreadPool.QueueUserWorkItem(HandleSentence, sentences);

            // Read & Add unfinsihed tasks
            var plans = entity.GetElements<UserPlan, NonMappingTable>(
                "tb_Tasks", new NonMappingTable());
            plans.ForEach(plan =>
            {
                plan.AddSign = ShowDetails;
                AddPlan(new(plan));
                LengthCalculation();
            });

            // Some alerts
            label_Sentence.Text = activation ? "MethodBox Aim" : "MethodBox Aim（评估副本）";
            string alert = GetSchedule(true);
            ScanTaskTime(alert);
            contextMenuStrip.Enabled = false;
        }

        #endregion

        #region 任务处理相关[F]
        internal void AddPlan(Plan task)
        {
            // Get serial
            var manager = Manager.SerialManager.GetManagerInstance();
            // Add to window
            task.Top = manager.AddTask(task);
            panel_M.Controls.Add(task);
            LengthCalculation();
        }

        protected void ScanTaskTime(string alert = "")
        {
            // Construct task list for alert window
            List<string> tasks = new List<string>();

            // Iterate controls which a symbol of tasks
            foreach (var item in panel_M.Controls)
            {
                Plan temp;

                // Other controls
                if (item is not Plan)
                {
                    continue;
                }
                else
                {
                    temp = item as Plan;
                }

                // Switch whether is handling
                if (temp.Status != "正在办")
                {
                    if (DateTime.Now.ToBinary() >= temp.StartTime)
                    {
                        tasks.Add(temp.Capital);
                    }
                }
            }

            // Exits specific task
            if (tasks.Count != 0)
            {
                new Alert(tasks, alert).ShowDialog();
            }
            tasks.Clear();
        }
        #endregion

        #region 加载器[F]
        protected void HoldList()
        {
            // Load list on panel
            panel_L.ControlAdded += new ControlEventHandler(Another_OnControlAdded);
            panel_L.Controls.Clear();

            // Load list name from Access
            var entity = AccessEntity.GetAccessEntityInstance();
            var taskLists = entity.GetElements<TaskList, NonMappingTable>(
             "tb_Lists", new NonMappingTable());

            // Prepare list of tasks
            List<string> list = new();
            taskLists.ForEach(t => list.Add(t.ListName));

            foreach (var item in list)
            {
                // Add parent node
                Function parentMain = new Function(item, item, 1);
                panel_L.Controls.Add(parentMain);

                // Get specific task object
                var taskList = entity.GetElement<UserPlan, NonMappingTable>(
                    new NonMappingTable(), "tb_Tasks", "Parent", item, true);

                // Iterate task list
                foreach (var task in taskList)
                {
                    Function sonMain = new Function(task.Capital, item, 0);
                    panel_L.Controls.Add(sonMain);
                }
            }

            // Unsubscribe event about length
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
                var officalInvoke = new Action<Plan>(AddPlan);
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
        }

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
        #region 每日一句相关[F]
        public async Task<List<string>> GetSentence()
        {
            // Build request to get sentences for server
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Add to array
                    var resultString = await httpClient.GetStringAsync("https://www.methodbox.top/Services/StonePlanner/sentence.txt");
                    return resultString.Split(';').ToList();
                }
                catch (HttpRequestException ex)
                {
                    // Add a common sentence
                    ErrorCenter.AddError(DataType.ExceptionsLevel.Caution, ex);
                    return new List<string>() { "浪费时间叫虚度，剥用时间叫生活。" };
                }
            }
        }

        internal void HandleSentence(object sentences)
        {
            // Get sentences array
            var sentencesInstance = (List<string>) sentences;
            var random = new Random();

            // Automatically change sentence
            while (true)
            {
                UISyncContext.Post((state) =>
                {
                    label_Sentence.Text = sentencesInstance[random.Next(0, sentencesInstance.Count)];
                }, null);
                Thread.Sleep(5_000);
            }
        }

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

        private void MenuClick(object sender, EventArgs e)
        {
            // Get name of menu
            var callerName = ((ToolStripMenuItem) sender).Name;

            // Get specific modifier
            string modifier = callerName.Split('_')[2];

            // Signal
            if (modifier.StartsWith("S"))
            {
                int signal = Convert.ToInt32(modifier.Substring(1));
                AddSignal(signal);
            }
            // Create window
            else
            {
                if (modifier == "Banned")
                {
                    MessageBox.Show("该功能目前不可用");
                    return;
                }
                var function = new Function(null, null, null);
                function._Name = modifier;
                function.Function_Click(this, null);
            }
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            PlugIn plugIn = new PlugIn();
            plugIn.Show();
        }
    }
}