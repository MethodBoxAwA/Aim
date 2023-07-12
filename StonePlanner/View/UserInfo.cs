using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace StonePlanner
{
    /// <summary>
    /// display user personal information
    /// </summary>
    public partial class UserInfo : MetroForm
    {
        /// <summary>
        /// format spaces
        /// </summary>
        public int Space { get; set; }

        /// <summary>
        /// initialize component
        /// </summary>
        public UserInfo()
        {
            InitializeComponent();
        }

        private void UserInfo_Load(object sender, EventArgs e)
        {
            // refuse display when the soft isn't activated
            if (!Main.activation)
            {
                new Activation().Show();
                label_Username.Text = $"用 户 名：需激活";
                label_Money.Text = $"金 币 数 量：需激活";
                label_LastingC.Text = $"耐力值：需激活";
                label_ExplosiveC.Text = $"爆发值：需激活";
                label_WisdomC.Text = $"智慧值：需激活";
                return;
            }
            // user's basic information
            label_Username.Text = $"用 户 名：{Login.UserName}";
            label_Money.Text = $"金 币 数 量：{Main.money}";
            // user's lasting information
            var Lasting = LevelGetter(Main.lasting);
            label_LastingC.Text = $"耐力值：{Main.lasting}"+
                Inner.InnerFuncs.MultipleStrings(Space- Lasting[0].
                ToString().Length) +$"Lv.{Lasting[0]}";
            label_Lastingleft.Text = Lasting[1].ToString();
            label_Lastingright.Text = Lasting[2].ToString();
            // user's lasting progress·
            int delta = Convert.ToInt32(label_Lastingright.Text) - Convert.ToInt32(label_Lastingleft.Text);
            int lasting = Main.lasting;
            panel_Lasting.Width = (int) (((double) (lasting - Convert.ToInt32(label_Lastingleft.Text)) / (double) delta) * 184);
            // user's explosive information
            var Explosive = LevelGetter(Main.explosive);
            label_ExplosiveC.Text = $"爆发值：{Main.explosive}" +
                Inner.InnerFuncs.MultipleStrings(Space - Explosive[0].
                ToString().Length) + $"Lv.{Explosive[0]}";
            label_Explosiveleft.Text = Explosive[1].ToString();
            label_Explosiveright.Text = Explosive[2].ToString();
            // user's explosive progress
            delta = Convert.ToInt32(label_Explosiveright.Text) - Convert.ToInt32(label_Explosiveleft.Text);
            int explosive = Main.explosive;
            panel_Explosive.Width = (int) (((double) (explosive - Convert.ToInt32(label_Explosiveleft.Text)) / (double) delta) * 184);
            // user's wisdom information
            var Wisdom = LevelGetter(Main.wisdom);
            label_WisdomC.Text = $"智慧值：{Main.wisdom}" +
               Inner.InnerFuncs.MultipleStrings(Space - Wisdom[0].
               ToString().Length) + $"Lv.{Wisdom[0]}";
            label_Wisdomleft.Text = Wisdom[1].ToString();
            label_Wisdomright.Text = Wisdom[2].ToString();
            // user's wisdom progress
            delta = Convert.ToInt32(label_Wisdomright.Text) - Convert.ToInt32(label_Wisdomleft.Text);
            int wisdom = Main.wisdom;
            panel_Wisdom.Width = (int) (((double) (wisdom - Convert.ToInt32(label_Wisdomleft.Text)) / (double) delta) * 184);
            // overall rating
            double point_User = lasting * 0.05 + wisdom * 0.02 + explosive * 0.01;
            if (point_User > 6)
            {
                point_User = 6d;
            }
            // refactoring score read
            var tasks = SQLConnect.SQLCommandQuery("SELECT * FROM Tasks",
                ref Main.odcConnection);
            /*
             * to avoid so many tasks be read
             * we suggests to read recent task
             * to calc user's rate
             */
            int i = 0,sum = 0;
            while (tasks.Read())
            {
                sum += Convert.ToInt32(tasks["TaskDiff"]);
                i++;
                if (i == 10) break;
            }
            double point_Plan;
            if (i == 0) 
            { 
                 point_Plan = 0;
            }
            else
            {
                point_Plan = sum / i;
            }
            label_Point.Text = $"评 分 值（pp）：{point_User + point_Plan:F2}";
        }

        /// <summary>
        /// get level information
        /// </summary>
        /// <param name="nowValue">now value</param>
        /// <returns>level information:
        /// [0] = Grade; [1] = left border; [2] = Right border.</returns>
        protected List<int> LevelGetter(int nowValue) 
        {
            int i = 0;
            int j = 0;
            int k = 0;
            for (i = 1; j < nowValue; i++)
            {
                j += 100 * i;
                k += 100 * (i - 1);
            }
            List<int> l = new List<int>();
            l.Add(i-1);
            l.Add(k);
            l.Add(j);
            return l;
        }
    }
}
