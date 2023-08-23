using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MetroFramework.Forms;
using StonePlanner.Classes.DataHandlers;
using StonePlanner.Classes.DataTypes;
using StonePlanner.Control;
using static StonePlanner.Classes.DataTypes.Structs;

namespace StonePlanner.View
{
    /// <summary>
    /// The window to add new plan
    /// </summary>
    public partial class AddTodo : MetroForm
    {
        /// <summary>
        /// SendMessage function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, 
            IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ReleaseCapture function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();
        /// <summary>
        /// the specific delegate to call plan add function
        /// </summary>
        /// <param name="plan">plan instance</param>
        public delegate void PlanAddInvoke(Plan plan);
        /// <summary>
        /// the specific delegate to call Signal add function
        /// </summary>
        public Action<int> Addsignal;
        /// <summary>
        /// PlanAddInvoke's instantiation concrete implementation
        /// </summary>
        PlanAddInvoke PlanAdditionInvoke;

        /// <summary>
        /// initialize component
        /// </summary>
        /// <param name="TargetFun">AddPlan's target callback function</param>
        /// <param name="Addsignal">Addsignal's target callback function</param>
        public AddTodo(PlanAddInvoke TargetFun,Action<int> Addsignal)
        {
            InitializeComponent();
            PlanAdditionInvoke = new PlanAddInvoke(TargetFun);
            this.Addsignal = Addsignal;
        }

        /// <summary>
        /// Add plan object according to plan struct
        /// </summary>
        /// <param name="planStruct"></param>
        internal AddTodo(PlanAddInvoke targetFun,Structs.PlanStruct planStruct)
        {
            InitializeComponent();
            PlanAdditionInvoke = new PlanAddInvoke(targetFun);
            //set default value to controls
            try
            {
                domainUpDown_Difficulty.Text = "SERVER" + " " + planStruct.Difficulty.ToString();
            }
            catch
            {
                domainUpDown_Difficulty.Text = "SERVER" + " " + "0.0";
            }
            try
            {
                textBox_Capital.Text = planStruct.Capital;
            }
            catch
            {
                textBox_Capital.Text = "无标题";
            }
            try
            {
                textBox_Explosive.Text = planStruct.Explosive.ToString();
            }
            catch
            {
                textBox_Explosive.Text = "0";
            }
            try
            {
                textBox_Intro.Text = planStruct.Intro;
            }
            catch
            {
                textBox_Intro.Text = "无简介";
            }
            try
            {
                textBox_Lasting.Text = planStruct.Lasting.ToString();
            }
            catch
            {
                textBox_Lasting.Text = "0";
            }
            try
            {
                textBox_Time.Text = planStruct.Seconds.ToString();
            }
            catch
            {
                textBox_Time.Text = "100";
            }
            try
            {
                textBox_Wisdom.Text = planStruct.Wisdom.ToString();
            }
            catch
            {
                textBox_Wisdom.Text = "0";
            }
        }

        /// <summary>
        /// window load function
        /// </summary>
        private void AddTodo_Load(object sender, EventArgs e)
        {
            //set topmost and some default settings
            this.TopMost = true;
            domainUpDown_Difficulty.ReadOnly = true;
            label_T.Text = "新建一个待办";
            metroButton_Submit.Text = "新建待办(&D)";
            textBox_Numbered.ReadOnly = true;
            //add now time to textbox
            textBox_hh.Text = DateTime.Now.ToString("HH");
            textBox_mm.Text = DateTime.Now.ToString("mm");
            //add difficulties level
            //NOTE:
            //to avoid precision issues caused by float
            //format string by "F1"
            for (double i = 0.1; i < 2.0; i += 0.1)
            {
                domainUpDown_Difficulty.Items.Add($"EASY {i:F1}");
            }
            for (double i = 2.0; i < 4.0; i += 0.1)
            {
                domainUpDown_Difficulty.Items.Add($"MIDDLE {i:F1}");
            }
            for (double i = 4.0; i < 6.0; i += 0.1)
            {
                domainUpDown_Difficulty.Items.Add($"HARD {i:F1}");
            }
            for (double i = 6.0; i < 9.0; i += 0.1)
            {
                domainUpDown_Difficulty.Items.Add($"DESPAIR {i:F1}");
            }
            for (double i = 9.0; i < 10.0; i += 0.1)
            {
                domainUpDown_Difficulty.Items.Add($"BEYOND {i:F1}");
            }

            //read exists list in DataBase
            var sResult = SQLConnect.SQLCommandQuery("SELECT * FROM Lists");
            while (sResult.Read())
            {
                comboBox_List.Items.Add(sResult[1]);
            }

            //add tips(deleted)
        }

        /// <summary>
        /// add plan function
        /// </summary>
        private void button_New_Click(object sender, EventArgs e)
        {
            try
            {
                //create plan class
                PlanStructC psc = new PlanStructC();
                //class encapsulation
                psc.Capital = textBox_Capital.Text;
                psc.Seconds = Convert.ToInt32(textBox_Time.Text);
                psc.Intro = textBox_Intro.Text;
                psc.Lasting = Convert.ToInt32(textBox_Lasting.Text);
                psc.Explosive = Convert.ToInt32(textBox_Explosive.Text);
                psc.Wisdom = Convert.ToInt32(textBox_Wisdom.Text);
                psc.Parent = comboBox_List.SelectedItem.ToString();
                DateTime _ = new DateTime(
                    dateTimePicker_Now.Value.Year,
                    dateTimePicker_Now.Value.Month,
                    dateTimePicker_Now.Value.Day,
                    Convert.ToInt32(textBox_hh.Text),
                    Convert.ToInt32(textBox_mm.Text),
                    0
                    );
                psc.UDID = new Random().Next(100000000, 999999999);
                psc.StartTime = _.ToBinary();
                psc.Addsignal = Addsignal;
                double diff = 0D;
                try
                {
                    diff = Math.Round(Convert.ToDouble(domainUpDown_Difficulty.SelectedItem.ToString().Split(' ')[1]),
                        1);
                }
                catch
                {
                    diff = 0D;
                }
                psc.Difficulty = diff;
                //invoke callback function
                PlanAdditionInvoke(new Plan(psc));
                Close();
            }
            catch (Exception ex)
            {
                //handle unknown error
                MessageBox.Show(ex.Message + "\n这通常是您错误的键入了某个值，或没有输入某个值导致。",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// move window
        /// </summary>>
        private void panel_Top_MouseDown(object sender, MouseEventArgs e)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 2;

            if (e.Button == MouseButtons.Left)  // 按下的是鼠标左键   
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr) HTCAPTION, IntPtr.Zero);// 拖动窗体  
            }
        }

        /// <summary>
        /// close window
        /// </summary>
        private void panel_Top_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// close window
        /// </summary>
        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        //arouse AddList window
        private void metroButton_Add_Click(object sender, EventArgs e)
        {
            AddList al = new AddList();
            al.Show();
        }
    }
}
