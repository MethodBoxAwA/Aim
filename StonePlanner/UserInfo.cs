using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using static StonePlanner.DataType.Structs;
using static StonePlanner.Interfaces;
using static StonePlanner.Manager;

namespace StonePlanner
{
    public partial class UserInfo : MetroForm
    {
        //空格数
        int space = 17;
        public UserInfo()
        {
            InitializeComponent();
        }

        private void UserInfo_Load(object sender, EventArgs e)
        {
            var moneyManager = Manager.MoneyManager.GetManagerInstance();
            var propertyManager = Manager.PropertyManager.GetManagerInstance();

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
            //用户基本信息
            var accountManager = AccountManager.GetManagerInstance();
            label_Username.Text = $"用 户 名：{accountManager.GetValue().Item1}";
            label_Money.Text = $"金 币 数 量：{moneyManager.GetValue()}";
            //耐力值信息
            var Lasting = LevelGetter(propertyManager.Lasting);
            label_LastingC.Text = $"耐力值：{propertyManager.Lasting}"+
                Helpers.TextHelper.MultipleStrings(space - Lasting[0].
                ToString().Length) +$"Lv.{Lasting[0]}";
            label_Lastingleft.Text = Lasting[1].ToString();
            label_Lastingright.Text = Lasting[2].ToString();
            //耐力值进度
            int delta = Convert.ToInt32(label_Lastingright.Text) - Convert.ToInt32(label_Lastingleft.Text);
            int lasting = propertyManager.Lasting;
            panel_Lasting.Width = (int) (((double) (lasting - Convert.ToInt32(label_Lastingleft.Text)) / (double) delta) * 184);
            //爆发值信息
            var Explosive = LevelGetter(propertyManager.Explosive);
            label_ExplosiveC.Text = $"爆发值：{propertyManager.Explosive}" +
                Helpers.TextHelper.MultipleStrings(space - Explosive[0].
                ToString().Length) + $"Lv.{Explosive[0]}";
            label_Explosiveleft.Text = Explosive[1].ToString();
            label_Explosiveright.Text = Explosive[2].ToString();
            //爆发值进度
            delta = Convert.ToInt32(label_Explosiveright.Text) - Convert.ToInt32(label_Explosiveleft.Text);
            int explosive = propertyManager.Explosive;
            panel_Explosive.Width = (int) (((double) (explosive - Convert.ToInt32(label_Explosiveleft.Text)) / (double) delta) * 184);
            //智慧值信息
            var Wisdom = LevelGetter(propertyManager.Wisdom);
            label_WisdomC.Text = $"智慧值：{propertyManager.Wisdom}" +
               Helpers.TextHelper.MultipleStrings(space - Wisdom[0].
               ToString().Length) + $"Lv.{Wisdom[0]}";
            label_Wisdomleft.Text = Wisdom[1].ToString();
            label_Wisdomright.Text = Wisdom[2].ToString();
            //智慧值进度
            delta = Convert.ToInt32(label_Wisdomright.Text) - Convert.ToInt32(label_Wisdomleft.Text);
            int wisdom = propertyManager.Wisdom;
            panel_Wisdom.Width = (int) (((double) (wisdom - Convert.ToInt32(label_Wisdomleft.Text)) / (double) delta) * 184);

            // Get user score
            var entity = AccessEntity.GetAccessEntityInstance();
            var taskList = entity.GetElements<UserPlan, NonMappingTable>(
                "tb_Tasks", new NonMappingTable());
            var pointPlan = 0d;
            var pointUser = lasting * 0.05 + wisdom * 0.02 + explosive * 0.01;
            var count = taskList.Count > 10 ? 10 : taskList.Count;

            for (int num = 0; num < count; num++)
            {
                pointPlan += taskList[num].Difficulty;
            }

            pointPlan = pointPlan / count;
            label_Point.Text = $"评 分 值（pp）：{pointUser + pointPlan:F2}";

        }

        /// <summary>
        /// 返回级别信息
        /// </summary>
        /// <param name="v">当前值</param>
        /// <returns>【0】= 等级；【1】= 左边界；【2】= 右边界。</returns>
        protected List<int> LevelGetter(int v) 
        {
            int i = 0;
            int j = 0;
            int k = 0;
            for (i = 1; j < v; i++)
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
