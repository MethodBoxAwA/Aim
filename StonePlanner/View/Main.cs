using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static StonePlanner.Develop.Sign;
using static StonePlanner.Exceptions;
using static StonePlanner.Structs;

/*
 * **************************************************************************
 * ********************                                  ********************
 * ********************      COPYRIGHT MethodBox 2022       *****************
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
    /// <summary>
    /// main window
    /// </summary>
    public partial class Main : Form
    {
        #region Primary fields
        // const
        const int DC_PLANHEIGHT = 36;
        const int DC_LRESULT = 0;
        // task list
        public Dictionary<int, Plan> TasksDict = new Dictionary<int, Plan>();
        // addSignal
        internal Signal signal = new Signal();
        // the request body object itself for outgoing request deletion
        internal static Plan plan = null;
        internal static List<string> sentence = new List<string>();
        internal static List<string> nownn = new List<string>();
        // array of discarded tasks
        public static List<Plan> recycle_bin = new List<Plan>();
        // TO-DO
        internal static PlanStructC planner; //It is a void*
        // time count
        internal static int nTime;
        // money and attribute
        internal static int money;
        internal static int lasting;
        internal static int explosive;
        internal static int wisdom;
        // database password
        internal static string password = "methodbox5";
        // check the language pack
        internal static bool activation = false;
        internal static bool banned = false;
        // global display
        TaskDetails td;
        // database queries
        internal static OleDbConnection odcConnection = new OleDbConnection();
        public static DateTime tStart;
        #endregion
        #region External references
        /// <summary>
        /// this function is used to send Windows Message (WM) handling window drag events.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// the function releases the mouse capture from the window
        /// in the current thread and resumes the usual mouse input processing.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();
        /// <summary>
        /// retrieves information about the specified process
        /// </summary>
        /// <returns></returns>
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);
        /// <summary>
        /// the function sends a drag event for dragging the window.
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
        #region Main window and settings load/exit
        /// <summary>
        /// <para>the main window constructor, the basic purpose is as follows:</para>
        /// <para>load controls (automatically generated)</para>
        /// <para>assign yourself to your own main variable</para>
        /// <para>cancel the cross-thread operation check</para>
        /// <para>load the setting window in advance to facilitate later reading of the settings</para>
        /// </summary>
        public Main()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            // delegate used for control addition
            _controlsAdd = new AddDelegate(FunctionLoader);
            Settings settings = new Settings();
            label_XHDL.Parent = pictureBox_Main;
            settings.Dispose();
            // check activation here, don't ask why
            var hresult = SQLConnect.SQLCommandQuery($"SELECT * FROM Users WHERE Username = 'mactivation'");
            hresult.Read();
            string code = hresult[4] as string;
            // check banned
            if (code == "Banned")
            {
                banned = true;
            }
            if (StonePlanner.License.Code.codes.Contains(code))
            {
                activation = true;
            }
            else
            {
                activation = false;
            }
            // bind Message Queuing to process event
            signal.SignChanged += HandleSign;
        }
        // add control delegate
        delegate void AddDelegate();
        // delegate instance
        readonly AddDelegate _controlsAdd;
        /// <summary>
        /// addSignal handler
        /// </summary>
        /// <param name="sender">object from which the call originated</param>
        /// <param name="e">addSignal event args</param>
        internal void HandleSign(object sender, DataType.SignChangedEventArgs e)
        {
            if (e.Sign == 1)
            {
                // connect to database
                string strConn = $" Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = {Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password}";
                OleDbConnection myConn = new OleDbConnection(strConn);
                myConn.Open();
                // search the database first
                //SELECT * FROM Persons WHERE City='Beijing'
                var hResult = SQLConnect.SQLCommandQuery($"SELECT * FROM Tasks WHERE ID = {plan.ID}");
                if (hResult.HasRows)
                {
                    /*
                     * Bugs here:
                     * when the task completes, it is cleaned up from the list
                     * save to the database should be saved when the task is deleted
                     * handle method：
                     * change the save location
                    */
                    string updateString = $"UPDATE Tasks SET TaskTime = {plan.Seconds}" +
                                $" , TaskStatus = \"已办完\"" +
                                $" WHERE ID = {plan.ID}";
                    SQLConnect.SQLCommandQuery(updateString, ref Main.odcConnection);
                }
                else
                {
                    // create insert expression
                    string strInsert = " INSERT INTO Tasks ( TaskName , TaskIntro , TaskStatus , " +
                        "TaskTime , TaskDiff ,TaskLasting ,TaskExplosive , TaskWisdom , ID" +
                        " , TaskParent) VALUES ( ";
                    strInsert += "'" + plan.Capital + "', '";
                    strInsert += plan.Intro + "', '";
                    strInsert += plan.status + "', ";
                    strInsert += plan.Seconds + ", ";
                    strInsert += plan.Difficulty + ",";
                    strInsert += plan.Lasting + ",";
                    strInsert += plan.Explosive + ",";
                    strInsert += plan.Wisdom + ",";
                    strInsert += plan.ID + ",";
                    strInsert += "'" + plan.Parent + "'" + ")";
                    // execute insert
                    OleDbCommand inst = new OleDbCommand(strInsert, myConn);
                    inst.ExecuteNonQuery();
                }
                // delete specific task
                int hNumber = plan.Lnumber;
                recycle_bin.Add(plan);
                panel_M.Controls.Remove(plan);
                TasksDict[hNumber] = null;
                plan = null;
                LengthCalculation();
                GC.Collect();
            }
            // [deprecated] Sign == 1 then add a task
            if (e.Sign == 2)
            {
                if (panel_L.Width <= 120)
                {
                    panel_L.Width += 2;
                    AddSignal(2);
                }
            }
            // controls the menu length
            else if (e.Sign == 3)
            {
                if (panel_L.Width > 0)
                {
                    panel_L.Width -= 2;
                    AddSignal(3);
                }
            }
            // [deprecated] Sign == 1 then export tasks
            else if (e.Sign == 5)
            {
                ExportTodo et = new ExportTodo(panel_M.Controls);
                et.Show();
            }
            // show task detail control
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
                td.StatusResult = plan.status;
                td.Difficulty = plan.Difficulty;
                td.Lasting = plan.Lasting.ToString();
                td.Explosive = plan.Explosive.ToString();
                td.Wisdom = plan.Wisdom.ToString();
                SoundPlayer sp = new SoundPlayer($@"{Application.StartupPath}\icon\Click.wav");
                sp.Play();
                panel_TaskDetail.Controls.Add(td);
                td.BringToFront();
            }
            // close task detail control
            else if (e.Sign == 7)
            {
                panel_TaskDetail.Controls.Remove(td);
                SoundPlayer sp = new SoundPlayer($@"{Application.StartupPath}\icon\Click.wav");
                sp.Play();
            }
            // addSignal == 8 is not exist
            // set picture invisible
            else if (e.Sign == 9)
            {
                pictureBox_Tip.Visible = false;
            }
            // get schedule information
            else if (e.Sign == 10)
            {
                // suspected problem: Repeat execution
                GetSchedule();
            }
            // addSignal == 11 is not exist
            // controls the window length
            else if (e.Sign == 12)
            {
                if (Width >= 256)
                {
                    Width -= 2;
                    pictureBox_T_Exit.Left -= 2;
                    AddSignal(12);
                }
            }
            // controls the window length
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
        /// overrides the form's message handler
        /// </summary>
        /// <param name="m">message</param>
        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case AM_EXIT:
                    Environment.Exit(0);
                    break;
                case AM_ADDMONEY:
                    MoneyUpdate(m.WParam.ToInt32());
                    break;
                case AM_GETMONEY:
                    var moneyQuery = SQLConnect.SQLCommandQuery($"SELECT * FROM Users WHERE Username = '{Login.UserName}'");
                    moneyQuery.Read();
                    money = Convert.ToInt32(moneyQuery.GetValue(2));
                    SendMessage(m.WParam, AM_GETMONEY, (IntPtr)money, IntPtr.Zero);
                    break;
                case AM_ADDTASK:
                    //TODO:操你妈傻逼内存，老子给你撂这儿，以后再他妈跟你玩
                    //AimLib.COPYDATASTRUCT cds = new AimLib.COPYDATASTRUCT();
                    //Type t = cds.GetType();
                    //cds = (MethodBox.AimLib.COPYDATASTRUCT)m.GetLParam(t);
                    //string strResult = cds.dwData.ToString() + ":" +cds.lpData;
                    //MessageBox.Show(strResult);
                    break;
                // call the base class function so that the system can process additional messages.
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// handles the user exit event and deposits new data that has not yet been stored.
        /// </summary>
        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            // save unfinished tasks
            foreach (var plan in TasksDict)
            {
                // determine whether it exists
                if (plan.Value != null)
                {
                     /*
                       * bugs here:
                       * when closing, check whether the corresponding task already exists
                       * if it exists, it is not added
                       * however, if the data has already been updated
                       * it cannot be stored, resulting in a bug in task restoration
                       * method:
                       * update only for the remaining time with a status of To Do and whether it is complete
                       */
                    string queryString = $"SELECT * FROM Tasks WHERE ID = {plan.Value.ID}";
                    var sqlResult = SQLConnect.SQLCommandQuery(queryString, ref Main.odcConnection);
                    if (sqlResult.HasRows)
                    {
                        // the corresponding task already exists, the query has been completed,
                        // otherwise the time is updated
                        sqlResult.Read();
                        // query Status is not pending
                        if (sqlResult["TaskStatus"].ToString() != "待办")
                        {
                            MessageBox.Show(sqlResult["TaskStatus"].ToString());
                            continue;
                        }
                        else
                        {
                            // update time and to-do status
                            if (plan.Value.Seconds > 0)
                            {
                                string updateString = $"UPDATE Tasks SET TaskTime = {plan.Value.Seconds}" +
                                    $" WHERE ID = {plan.Value.ID}";
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
                    string strInsert = "INSERT INTO Tasks ( TaskName , TaskIntro , TaskStatus , TaskTime , TaskDiff ,TaskLasting ,TaskExplosive , TaskWisdom , TaskParent , StartTime) VALUES ( ";
                    strInsert += "'" + plan.Value.Capital + "', '";
                    strInsert += plan.Value.Intro + "', '";
                    strInsert += plan.Value.status + "', ";
                    strInsert += plan.Value.Seconds + ", ";
                    strInsert += plan.Value.Difficulty + ",";
                    strInsert += plan.Value.Lasting + ",";
                    strInsert += plan.Value.Explosive + ",";
                    strInsert += plan.Value.Wisdom + ",";
                    strInsert += "'" + plan.Value.Parent + "',";
                    strInsert += "'" + plan.Value.StartTime.ToBinary() + "')";
                    // execute insert
                    SQLConnect.SQLCommandExecution(strInsert, ref Main.odcConnection);
                    recycle_bin.Add(plan.Value);
                }
            }
            // close database
            Main.odcConnection.Close();
            Environment.Exit(0);
        }
        /// <summary>
        /// this function is used for main window loading
        /// </summary>
        private void Main_Load(object sender, EventArgs e)
        {
            #region Window loads
            this.TopMost = false;
            // difficulty evaluation
            // initialize the Recycle Bin first
            Recycle recycleBin = new Recycle();
            GC.Collect();
            // get money from the table
            // Login.UserName = "Me";
            var moneyTemp = SQLConnect.SQLCommandQuery($"SELECT * FROM Users WHERE Username = '{Login.UserName}'");
            moneyTemp.Read();
            money = Convert.ToInt32(moneyTemp.GetValue(2));
            // read individual properties
            lasting = Convert.ToInt32(moneyTemp.GetValue(6));
            explosive = Convert.ToInt32(moneyTemp.GetValue(7));
            wisdom = Convert.ToInt32(moneyTemp.GetValue(8));
            label_GGS.Text = moneyTemp.GetValue(2).ToString();
            Thread valueThread = new Thread(new ThreadStart(ValueGetter));
            valueThread.Start();
            // pictureBox_Main.ImageLocation = "https://tse1-mm.cn.bing.net/th/id/R-C.2fd0dadf9d13c716cf0494d17875cf3b?rik=mf3ZQjupoBDr2A&riu=http%3a%2f%2fup.36992.com%2fpic%2f07%2fd3%2fe8%2f07d3e81f37f5922b5b0021a1c0b2d3da.jpg&ehk=P8hpii3cUJykmCt97WX0kATyROzUNRuexj8faXE7q6c%3d&risl=&pid=ImgRaw&r=0";
            // get the motto
            Thread sentenceGetter = new Thread(() => GetDailySentence());
            sentenceGetter.Start();
            // assignment empty plan
            for (int i = 0; i < 100; i++)
            {
                PlanStructA emptyPlan = new()
                {
                    Capital = "NULL",
                    Parent = null,
                    StartTime = 0,
                    Wisdom = 0,
                    Lasting = 0,
                    Explosive = 0,
                    Intro = "NULL",
                    Seconds = 0,
                    Addsignal = (Action<int>) AddSignal
                };
                Plan p = new(emptyPlan)
                {
                    Lnumber = -1
                };
                TasksDict.Add(i, null);
            }
            //load datetime
            label_Date.Text = DateTime.Now.ToString("dd");
            label_Month.Text = DateTime.Now.ToString("MM");
            #endregion
            Thread loaderThread = new Thread(new ThreadStart(FunctionLoader));
            loaderThread.Start();
            CheckForIllegalCrossThreadCalls = false;
            #region Incomplete task read
            for (int i = 0; i < recycleBin.dataGridView1.Rows.Count - 1; i++)
            {
                if (recycleBin.dataGridView1.Rows[i].Cells[5].Value.ToString() == "0")
                {
                    continue;
                }
                // a terrible override
                PlanStructB recycledPlan = new()
                {
                    Capital = recycleBin.dataGridView1.Rows[i].Cells[1].Value.ToString(),
                    Intro = recycleBin.dataGridView1.Rows[i].Cells[2].Value.ToString(),
                    Seconds = Convert.ToInt32(recycleBin.dataGridView1.Rows[i].Cells[5].Value),
                    Difficulty = Convert.ToDouble(recycleBin.dataGridView1.Rows[i].Cells[4].Value),
                    UDID = Convert.ToInt32(recycleBin.dataGridView1.Rows[i].Cells[0].Value),
                    Lasting = Convert.ToInt32(recycleBin.dataGridView1.Rows[i].Cells[6].Value),
                    Explosive = Convert.ToInt32(recycleBin.dataGridView1.Rows[i].Cells[7].Value),
                    Wisdom = Convert.ToInt32(recycleBin.dataGridView1.Rows[i].Cells[8].Value),
                    StartTime = Convert.ToInt64(recycleBin.dataGridView1.Rows[i].Cells[10].Value),
                    Addsignal = (Action<int>)AddSignal
                };
                AddPlan(new Plan(recycledPlan));
                LengthCalculation();
                plan = null;
            }
            #endregion
            // use linq to handle some format error
            sentence.FindAll(sentence => 
            sentence.Contains("\n")).ForEach(sentence => sentence.Replace("\n", ""));
            #region Function controller
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
            // scan time
            ScanTaskTime(alert);
            contextMenuStrip.Enabled = false;
        }
        #endregion
        #region Plan addition related

    
        /// <summary>
        /// invoke for plan addition
        /// </summary>
        /// <param name="plan">plan instance</param>
        delegate void PlanAddInvoke(Plan plan);
        #region Task processing related
        /// <summary>
        /// function to add plan to GUI
        /// </summary>
        /// <param name="pValue"></param>
        internal void AddPlan(Plan pValue)
        {
            // assign a unique number
            int thisNumber = -1;
            for (int i = 0; i < 100; i++)
            {
                if (TasksDict[i] == null)
                {
                    thisNumber = i;
                    break;
                }
            }
            if (thisNumber == -1) { return; }
            pValue.Top = 36 * thisNumber;
            // add to dict
            TasksDict[thisNumber] = pValue;
            panel_M.Controls.Add(pValue);
            LengthCalculation();
        }

        /// <summary>
        /// scan task time and switch whether is not finished
        /// </summary>
        /// <param name="alert">alert info</param>
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
                if (temp.status != "正在办")
                {
                    if (DateTime.Now >= temp.StartTime)
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
        #endregion
        #region Loader
        /// <summary>
        /// load user lists
        /// </summary>
        protected void HoldList()
        {
            // adds a new Controls.Add function to the
            // temporary storage sub control
            panel_L.ControlAdded += new ControlEventHandler(Another_OnControlAdded);
            panel_L.Controls.Clear();
            List<string> listLists = new List<string>();
            var sResult = SQLConnect.SQLCommandQuery("SELECT * FROM Lists", ref Main.odcConnection);
            while (sResult.Read())
            {
                listLists.Add(sResult[1].ToString());
            }
            /* for all lists Search for their child values in turn
             * there are too many reads here, so there is no need
             * to encapsulate the query
             * reference closes the original database connection
             */
            List<Plan> sonTask = new List<Plan>();
            //猜猜DataReader在哪儿，小子
            foreach (var item in listLists)
            {
                // add a parent node
                Function parentMain = new Function(item, item, 1);
                panel_L.Controls.Add(parentMain);
                try
                {
                    sResult = SQLConnect.SQLCommandQuery($"SELECT * FROM Tasks WHERE TaskParent = '{item}'");
                }
                catch { return; }
                // create a Plan object
                // 1 5 2 4 9 6 7 8
                while (sResult.Read())
                {
                    PlanStructB psb = new()
                    {
                        Capital = sResult[1].ToString(),
                        Intro = sResult[2].ToString(),
                        Seconds = Convert.ToInt32(sResult[5]),
                        Difficulty = Convert.ToInt64(sResult[4]),
                        UDID = Convert.ToInt32(sResult[0]),
                        Lasting = Convert.ToInt32(sResult[6]),
                        Explosive = Convert.ToInt32(sResult[7]),
                        Wisdom = Convert.ToInt32(sResult[8]),
                        Addsignal = (Action<int>) AddSignal
                    };
                    using Plan plan = new Plan
                    (
                          psb
                    );
                    Function sonMain = new Function(sResult[1].ToString(), item, 0); panel_L.Controls.Add(sonMain);
                }
            }
            panel_L.ControlAdded -= Another_OnControlAdded;
            return;
        }

        /// <summary>
        /// load 
        /// </summary>
        protected void FunctionLoader()
        {
            int i = 34;
            // load function (34 height)
            if (this.InvokeRequired)
            {
                this.Invoke(_controlsAdd);
            }
            else
            {
                AddTodo.PlanAddInvoke officalInvoke = new AddTodo.PlanAddInvoke(AddPlan);
                Function newTodo = new Function($"{Application.StartupPath}\\icon\\new.png",
                    $"新建任务", "__New__", officalInvoke, (Action<int>) AddSignal)
                {
                    Top = 0
                };
                panel_L.Controls.Add(newTodo);
                //Function export = new Function($"{Application.StartupPath}\\icon\\export.png", $"{langInfo[49]}", "__Export__");
                //export.Top = 34;
                //panel_L.Controls.Add(export);
                Function recycle = new($"{Application.StartupPath}\\icon\\recycle.png", "任务回收", "__Recycle__")
                {
                    Top = i
                };
                panel_L.Controls.Add(recycle);
                Function debugger = new($"{Application.StartupPath}\\icon\\debug.png",
                    $"调试工具", "__Debugger__", (Action<int>) AddSignal)
                {
                    Top = 7 * i
                };
                panel_L.Controls.Add(debugger);
                Function info = new($"{Application.StartupPath}\\icon\\info.png", "关于软件", "__Infomation__")
                {
                    Top = 9 * i
                };
                panel_L.Controls.Add(info);
                Function console = new($"{Application.StartupPath}\\icon\\console.png", "主控制台", "__Console__")
                {
                    Top = 3 * i
                };
                panel_L.Controls.Add(console);
                Function IDE = new($"{Application.StartupPath}\\icon\\program.png", "事件编写", "__IDE__")
                {
                    Top = 4 * i
                };
                panel_L.Controls.Add(IDE);
                Function Online = new($"{Application.StartupPath}\\icon\\server.png", $"在线协作", "__Online__")
                {
                    Top = 5 * i
                };
                panel_L.Controls.Add(Online);
                //Function Settings = new($"{Application.StartupPath}\\icon\\settings.png", $"软件设置", "__Settings__")
                //{
                //    Top = 6 * i
                //};
                //panel_L.Controls.Add(Settings);
                Function Shop = new($"{Application.StartupPath}\\icon\\shop.png", $"我的商城", "__Shop__")
                {
                    Top = 2 * i
                };
                panel_L.Controls.Add(Shop);
                Function Schedule = new($"{Application.StartupPath}\\icon\\schedule.png",
                    $"排班日历", "__Schedule__", (Action<int>) AddSignal)
                {
                    Top = 8 * i
                };
                panel_L.Controls.Add(Schedule);
                Function Update = new($"{Application.StartupPath}\\icon\\update.png",$"软件升级", "__Update__")
                {
                    Top = 6 * i
                };
                panel_L.Controls.Add(Update);
                // guess where the click function is?
                // I didn't expect it, here！
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
                // resting state
                label_Status.Text = "正在休息";
            }
            //return;
        }
        #endregion
        #region Anti-piracy and process related

 
        /// <summary>
        /// anti-piracy check
        /// </summary>
        private void timer_Anti_Tick(object sender, EventArgs e)
        {
            if (Process.GetCurrentProcess().Parent().ProcessName != "explorer.exe" && Process.GetCurrentProcess().Parent().ProcessName != "msvsmon")
            {
                string p = "";
                try
                {
                    p = Process.GetCurrentProcess().Parent().ProcessName.Split('.')[0];
                }
                catch
                {
                    p = Process.GetCurrentProcess().Parent().ProcessName;
                }
                if (p == "explorer") return;
                MessageBox.Show($"您可能试图尝试在其它框架下运行Aim，例如{p}。请注意，这样的做法不是正确的。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SQLConnect.SQLCommandExecution("INSERT INTO Users(UserName) Values('METHODBOX_BAN')", ref Main.odcConnection);
                // backhand closes threads
                label_Sentence.Text = "A Fetal Error Occured";
                timer_Anti.Enabled = false;
                return;
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
            }
            catch
            {
                MessageBox.Show("您可能试图尝试在其它框架下运行Aim，例如BepInEx。请注意，这样的做法不是正确的。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SQLConnect.SQLCommandExecution("INSERT INTO Users(UserName) Values('METHODBOX_BAN')", ref Main.odcConnection);
                // backhand closes threads
                label_Sentence.Text = "A Fetal Error Occured";
                return;
            }
            if (files.Length != 0)
            {
                SQLConnect.SQLCommandExecution("INSERT INTO Users(UserName) Values('METHODBOX_BAN')", ref Main.odcConnection);
                // backhand closes threads
                label_Sentence.Text = "A Fetal Error Occured";
                //int isCritical = 1;  // we want this to be a Critical Process
                //int BreakOnTermination = 0x1D;  // value for BreakOnTermination (flag)
                //Process.EnterDebugMode();  //acquire Debug Privileges
                //// setting the BreakOnTermination = 1 for the current process
                //NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermination, ref isCritical, sizeof(int));
                ////for (int i = 0; ; i++) { System.Console.WriteLine(i); }
            }
            try
            {
                var result = SQLConnect.SQLCommandQuery($"SELECT * FROM Users where Username='METHODBOX_BAN';");
                result.Read();
                if (result[0].ToString() != "" || result[0].ToString() != null)
                {
                    Ban ban = new Ban();
                    Opacity = 0;
                    // we want this to be a Critical Process
                    int isCritical = 1;
                    // value for BreakOnTermination (flag)
                    int breakOnTermination = 0x1D;
                    // acquire Debug Privileges
                    // setting the BreakOnTermination = 1 for the current process
                    Process.EnterDebugMode();  
                                               
                    NtSetInformationProcess(Process.GetCurrentProcess().Handle, 
                        breakOnTermination, 
                        ref isCritical, 
                        sizeof(int));
                    //Process.LeaveDebugMode();
                }
            }
            catch { }
        }

        /// <summary>
        /// determine whether the process contains this string (fuzzy)
        /// </summary>
        /// <param name="strProcName">process string</param>
        /// <returns>whether it contains</returns>
        public bool SearchProcA(string strProcName)
        {
            try
            {
                foreach (Process p in Process.GetProcesses())
                {

                    if (p.ProcessName.IndexOf(strProcName) > -1) 
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        #endregion
        #region Money and attribute operations
        /// <summary>
        /// change user money
        /// </summary>
        /// <param name="value">money value</param>
        internal void ChangeMoney(int value)
        {
            money += value;
            // inserts to the specified user
            SQLConnect.SQLCommandExecution($"UPDATE Users SET Cmoney = {money} WHERE Username = {Login.UserName}", ref Main.odcConnection);
        }
        /// <summary>
        /// update user money
        /// </summary>
        /// <param name="delta">money delta value</param>
        internal static void MoneyUpdate(int delta)
        {
            money += delta;
            SQLConnect.SQLCommandExecution($"UPDATE Users SET Cmoney = {money} WHERE Username = '{Login.UserName}';", ref Main.odcConnection);
        }

        /// <summary>
        /// update user attribute
        /// </summary>
        /// <param name="type">attribute type</param>
        /// <param name="delta">attribute delta value</param>
        internal static void ValuesUpdate(uint type, int delta)
        {
            if (type == 1)
            {
                lasting += delta;
                SQLConnect.SQLCommandExecution($"UPDATE Users SET ABT_lasting = {lasting} WHERE Username = '{Login.UserName}';", ref Main.odcConnection);
            }
            else if (type == 2)
            {
                explosive += delta;
                SQLConnect.SQLCommandExecution($"UPDATE Users SET ABT_explosive = {explosive} WHERE Username = '{Login.UserName}';", ref Main.odcConnection);
            }
            else if (type == 3)
            {
                wisdom += delta;
                SQLConnect.SQLCommandExecution($"UPDATE Users SET ABT_wisdom = {wisdom} WHERE Username = '{Login.UserName}';", ref Main.odcConnection);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// update user's all attribute
        /// </summary>
        /// <param name="lasting">lasting value</param>
        /// <param name="explosive">explosive value</param>
        /// <param name="wisdom">wisdom value</param>
        internal static void ValuesUpdate(int lasting, int explosive, int wisdom)
        {
            Main.lasting += lasting;
            Main.explosive += explosive;
            Main.wisdom += wisdom;

            SQLConnect.SQLCommandExecution($"UPDATE Users SET ABT_lasting = {Main.lasting} , ABT_explosive = {Main.explosive} , ABT_wisdom = {Main.wisdom} WHERE Username = '{Login.UserName}';", ref Main.odcConnection);
        }

        /// <summary>
        /// get user money
        /// </summary>
        public void ValueGetter()
        {
            while (true)
            {
                label_GGS.Text = money.ToString();
                Thread.Sleep(1000);
            }
        }
        #endregion
        #region Load the shift calendar
        internal string GetSchedule(bool @out = false)
        {
            Dictionary<DateTime, string> returns = new Dictionary<DateTime, string>();
            // inbuilt
            foreach (var item in panel_M.Controls)
            {
                if (item is Plan)
                {
                    DateTime d = (item as Plan).StartTime;
                    if (returns.ContainsKey(d))
                    {
                        // frequency statistics
                        continue;
                    }
                    else
                    {
                        Plan plan1 = (item as Plan);
                        string sch = plan1.StartTime.Hour switch
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
        #region Signal system
        /// <summary>
        /// add specific addSignal
        /// </summary>
        /// <param name="addSignal">signal you want to add</param>
        internal void AddSignal(int addSignal)
        {
            this.signal.AddSignal(addSignal);
        }

        /// <summary>
        /// control window length
        /// </summary>
        private void pictureBox_T_More_Click(object sender, EventArgs e)
        {
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
        #region Daily sentence/image loader
        /// <summary>
        /// get daily sentence
        /// </summary>
        public void GetDailySentence()
        {
            try
            {
                WebClient MyWebClient = new WebClient
                {
                    Credentials = CredentialCache.DefaultCredentials
                };
                //Gets or sets the network credentials used to authenticate requests to Internet resources
                Byte[] pageData = MyWebClient.DownloadData("https://lzr2006.github.io/wkgd/Services/StonePlanner/sentence.txt"); //下载                                                                                            //string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句
                foreach (var item in pageHtml.Split(';'))
                {
                    sentence.Add(item);
                }
            }
            catch (Exception ex)
            {
                // add internal sentence
                ErrorCenter.AddError(DataType.ExceptionsLevel.Caution, ex);
                sentence.Add("浪费时间叫虚度，剥用时间叫生活。");
            }
            return;
        }
        #endregion
        #region One sentence per day/one picture actuator
        /// <summary>
        /// change sentence
        /// </summary>
        private void label_Sentence_TextChanged(object sender, EventArgs e)
        {
            label_Sentence.Text = label_Sentence.Text.Replace("\n", "");
        }
        #endregion
        #region Appearance control
        /// <summary>
        /// move window
        /// </summary>
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

        /// <summary>
        /// show error center
        /// </summary>
        private void User_Picture_DoubleClick(object sender, EventArgs e)
        {
            ErrorCenter _ = new ErrorCenter();
            _.Show();
        }

        /// <summary>
        /// stretch the menu bar
        /// </summary>
        private void label_L_Function_Click(object sender, EventArgs e)
        {
            MouseWheel -= panel_L_MouseWheel;
            panel_L.Controls.Clear();
            FunctionLoader();
        }

        /// <summary>
        /// display a list of categories
        /// </summary>
        private void label_L_Type_Click(object sender, EventArgs e)
        {
            // add mouse scroll events
            MouseWheel += panel_L_MouseWheel;
            panel_L.Controls.Clear();
            // add the data that is done
            HoldList();
        }

        /// <summary>
        /// The middle mouse button of the category box
        /// scrolls in response to events
        /// </summary>
        protected void panel_L_MouseWheel(object sender, MouseEventArgs e)
        {
            // At? Absence?
            Rectangle pnlRightRectToForm1 = this.panel_L.ClientRectangle; // 获得Panel的矩形区域
            pnlRightRectToForm1.Offset(this.panel_L.Location);
            if (!pnlRightRectToForm1.Contains(e.Location)) return;
            // scroll up
            if (e.Delta > 0) 
            {

                if (panel_L.Top >= DC_LRESULT)
                {
                    panel_L.Top = DC_LRESULT;
                    // restore bottom control
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
                // dynamic extension
                panel_L.Height += DC_PLANHEIGHT;
                panel_L.Top += DC_PLANHEIGHT;
            }
            // scroll down
            else
            {
                panel_L.Height += DC_PLANHEIGHT;
                panel_L.Top -= DC_PLANHEIGHT;
            }
            // remove bottom button
            foreach (var item in panel_L.Controls.Find("buttom", true))
            {
                panel_L.Controls.Remove(item);
            }
            panel_L.BringToFront();
        }

        /// <summary>
        /// dynamically calculating box length
        /// </summary>
        internal void LengthCalculation()
        {
            int count = default;
            foreach (var item in TasksDict)
            {
                if (item.Value != null)
                {
                    count++;
                }
            }
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

        /// <summary>
        /// overwrite the add control event to add it in order
        /// </summary>
        protected void Another_OnControlAdded(object sender, ControlEventArgs e)
        {
            // the menu below is not considered
            if (e.Control.GetType() == typeof(Bottom))
                return;
            // get existing control ,height 34
            int i = panel_L.Controls.Count;
            e.Control.Top = (i - 1) * 34;
            // callback base class primitive function adding control
            base.OnControlAdded(e);
        }
        #endregion
        #region Right click menu events
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
            MessageBox.Show("信号控制功能已从AimPlanner中被移除","被移除的功能",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
        #region Other click events
        private void pictureBox_T_Float_DoubleClick(object sender, EventArgs e)
        {
            contextMenuStrip.Enabled = !contextMenuStrip.Enabled;
        }

        private void User_Piicture_Click(object sender, EventArgs e)
        {
            //TODO:我超！爆了！
            UserInfo info = new UserInfo();
            info.Show();
        }

        private void pictureBox_T_Float_Click(object sender, EventArgs e)
        {
            Update update = new Update();
            update.Show();
        }
        #endregion
    }
}