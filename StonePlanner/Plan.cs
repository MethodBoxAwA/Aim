using System;
using System.Drawing;
using System.Windows.Forms;
using static StonePlanner.DataType.Structs;

namespace StonePlanner
{
    public partial class Plan : UserControl
    {
        /// <summary>
        /// The capital of this plan
        /// </summary>
        public string Capital { get; set; }

        /// <summary>
        /// The remain seconds of this plan
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Task automatic numbering value
        /// </summary>
        public int Serial { get; set; }

        /// <summary>
        /// Current status of the task
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The introduce of this plan
        /// </summary>
        public string Intro { get; set; }

        /// <summary>
        /// A number from 1 to 10, in steps of 0.1, representing the difficulty of this plan
        /// </summary>
        public double Difficulty { get; set; }

        /// <summary>
        /// Lasting obtained from completing this plan
        /// </summary>
        public int Lasting { get; set; }

        /// <summary>
        /// Wisdom obtained from completing this plan
        /// </summary>
        public int Wisdom { get; set; }

        /// <summary>
        /// Explosive obtained from completing this plan
        /// </summary>
        public int Explosive { get; set; }

        /// <summary>
        /// Task unique identifier
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// List of Tasks Belonging to
        /// </summary>
        public new string Parent { get; set; }

        /// <summary>
        /// Task start time
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// Delegate for add plan to main window
        /// </summary>
        public Action<int> AddSign;

        public Action<Plan> ShowDetailsHandler;

        internal Plan(UserPlan planData)
        {
            // Initial controls
            InitializeComponent();

            // Build structure
            switch (planData.BuildMode)
            {
                case PlanBuildMode.A:
                    Parent = planData.Parent;
                    StartTime = Convert.ToInt64(planData.StartTime);
                    ShowDetailsHandler = planData.AddSign;
                    ID = planData.ID;
                    goto default;
                case PlanBuildMode.B:
                    Parent = planData.Parent;
                    StartTime = Convert.ToInt64(planData.StartTime);
                    ShowDetailsHandler = planData.AddSign;
                    ID = planData.ID;
                    goto default;
                case PlanBuildMode.C:
                    goto default;
                default:
                    Capital = planData.Capital;
                    Seconds = planData.Seconds;
                    Intro = planData.Intro;
                    Difficulty = planData.Difficulty;
                    Lasting = planData.Lasting;
                    Explosive = planData.Explosive;
                    Wisdom = planData.Wisdom;
                    break;
            }
        }
    

        private void Plan_Load(object sender, EventArgs e)
        {
            // Load default settings
            label_TaskDes.Text = Capital;
            button_Finish.Text = "完成";
            label_Time.Text = Seconds.ToString();
            this.timer1.Enabled = true;
        }

        private void panel_Status_Click(object sender, EventArgs e)
        {
            if (panel_Status.BackColor == Color.LightGray)
            {
                panel_Status.BackColor = Color.Red;
                Status = "正在办";
            }
            else 
            {
                panel_Status.BackColor = Color.LightGray;
                Status = "待办";
            }
           
        }
        protected override void CreateHandle()
        {
            if (!IsHandleCreated)
            {
                try
                {
                    base.CreateHandle();
                }
                catch (Exception ex) 
                {
                    ErrorCenter.AddError(DataType.ExceptionsLevel.Error, ex); 
                }
                finally
                {
                    if (!IsHandleCreated)
                    {
                        base.RecreateHandle();
                    }
                }
            }
        }

        private void button_Finish_Click(object sender, EventArgs e)
        {
            button_Finish.Enabled = false;
            //更新金钱
            //添加限制条件：只有任务完成时才可以被删除
            if (this.Seconds == 0)
            {
                var moneyManager = Manager.MoneyManager.GetManagerInstance();
                moneyManager.Change(+(int) this.Difficulty * 10);
                // Update properties
                var propertyManager = Manager.PropertyManager.GetManagerInstance();
                propertyManager.Change((+Lasting, +Explosive, +Wisdom));
                Main.plan = this;
                AddSign?.Invoke(1);
            }
            else
            {
                // Task has not been finished
                MessageBox.Show("任务尚未完成！", "尚未完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button_Finish.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Seconds <= 0)
            {
                Seconds = 0;
                panel_Status.BackColor = Color.Lime;
                Status = "已办完";
            }
            if (panel_Status.BackColor == Color.Red)
            {
                Seconds -= 1;
            }
            label_Time.Text = Seconds.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Seconds <= 0)
            {
                Seconds = 0;
                panel_Status.BackColor = Color.Lime;
                Status = "已办完";
            }
            if (panel_Status.BackColor == Color.Red)
            {
                Seconds -= 1;
            }
            label_Time.Text = Seconds.ToString();
        }

        private void label_TaskDes_Click_1(object sender, EventArgs e)
        {
            ShowDetailsHandler?.Invoke(this);
        }
    }
}