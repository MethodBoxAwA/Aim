using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static StonePlanner.DataType.Structs;
using MetroFramework.Forms;

namespace StonePlanner
{
    public partial class AddTodo : MetroForm
    {
        public Action<Plan> Addsignal;
        Action<Plan> PlanAdditionInvoke;

        public AddTodo(Action<Plan> TargetFun,Action<Plan> Addsignal)
        {
            InitializeComponent();
            PlanAdditionInvoke = new Action<Plan>(TargetFun);
            this.Addsignal = Addsignal;
        }

        internal AddTodo(PlanStruct planStruct)
        {
            InitializeComponent();

            try
            {
                domainUpDown_Difficulty.Text = "SERVER" + " " + planStruct.Difficulty.ToString();
                textBox_Capital.Text = planStruct.Capital;
                textBox_Explosive.Text = planStruct.Explosive.ToString();
                textBox_Intro.Text = planStruct.Intro;
                textBox_Lasting.Text = planStruct.Lasting.ToString();
                textBox_Time.Text = planStruct.Seconds.ToString();
                textBox_Wisdom.Text = planStruct.Wisdom.ToString();
            }

            catch (NullReferenceException) when (planStruct.Capital is null)
            {

                textBox_Capital.Text = "无标题";
            }
            catch (NullReferenceException) when (planStruct.Intro is null)
            {

                textBox_Intro.Text = "无标题";
            }
        }

        private void AddTodo_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
            domainUpDown_Difficulty.ReadOnly = true;
            label_T.Text = "新建一个待办";
            metroButton_Submit.Text = "新建待办(&D)";
            textBox_Numbered.ReadOnly = true;
            //Default HH and mm
            //Default MotherFucker
            textBox_hh.Text = DateTime.Now.ToString("HH");
            textBox_mm.Text = DateTime.Now.ToString("mm");
            //难度添加
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

            // Read task lists
            var entity = AccessEntity.GetAccessEntityInstance();
            var taskLists = entity.GetElements<TaskList, NonMappingTable>(
                "tb_Lists", new NonMappingTable());
            
            // Add to combo box
            foreach (var taskList in taskLists)
            {
                comboBox_List.Items.Add(taskList.ListName);
            }

            //加载TIPS
            try
            {
                //WFJsonStructure.DataItem weather;
                //Main.wf.GetWInfo(out weather);
                //if (Convert.ToInt32(weather.air) >= 180)
                //{
                //    //TIPS：今天天气状态良好，可以做想做的事情。
                //    label_Tips.Text = $"TIPS:空气较差（{weather.air}），不建议在外活动。";
                //}
                //else if (Convert.ToInt32(weather.uvIndex) > 6)
                //{
                //    label_Tips.Text = $"TIPS:今日紫外线较强（{weather.uvIndex}），请做好防护。";
                //}
            }
            catch
            {
                //天气预报 not loaded (x)
                //developer should be fucked (√)夜班c
            }
        }

        private void button_New_Click(object sender, EventArgs e)
        {
            try
            {
                UserPlan userPlan = new()
                {
                    Capital = textBox_Capital.Text,
                    Seconds = Convert.ToInt32(textBox_Time.Text),
                    Intro = textBox_Intro.Text,
                    Lasting = Convert.ToInt32(textBox_Lasting.Text),
                    Explosive = Convert.ToInt32(textBox_Explosive.Text),
                    Wisdom = Convert.ToInt32(textBox_Wisdom.Text),
                    Parent = comboBox_List.SelectedItem.ToString(),
                    StartTime = new DateTime(
                    dateTimePicker_Now.Value.Year,
                    dateTimePicker_Now.Value.Month,
                    dateTimePicker_Now.Value.Day,
                    Convert.ToInt32(textBox_hh.Text),
                    Convert.ToInt32(textBox_mm.Text),
                    0
                    ).ToBinary().ToString(),
                    ID = new Random().Next(100000000, 999999999),
                    AddSign = Addsignal
                };
                double diff = 0D;
                try
                {
                    diff = Math.Round(Convert.ToDouble(domainUpDown_Difficulty.SelectedItem.ToString().Split(' ')[1]), 1);
                }
                catch { diff = 0D; }
                userPlan.Difficulty = diff;

                //对指针传出
                PlanAdditionInvoke(new Plan(userPlan));
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n这通常是您错误的键入了某个值，或没有输入某个值导致。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel_Top_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }

        private void pictureBox_T_Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void groupBox_Area1_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void metroButton_Add_Click(object sender, EventArgs e)
        {
            AddList al = new AddList();
            al.Show();
        }
    }
}
