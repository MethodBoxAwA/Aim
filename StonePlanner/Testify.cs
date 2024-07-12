using MetroFramework.Forms;
using System;
using System.Windows.Forms;

namespace StonePlanner
{
    public partial class Testify : MetroForm
    {
        Manager.MoneyManager _moneyManager;
        Manager.PropertyManager _propertyManager;
        Manager.AccountManager _accountManager;

        public Testify()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = _accountManager!.GetValue().Item1;
            label3.Text = $"登录类型：{_accountManager!.GetValue().Item2}";
            label4.Text = $"用户金钱：{_moneyManager.GetValue()}";
            label5.Text = $"用户耐力值：{_propertyManager.Lasting}";
            label6.Text = $"用户爆发值：{_propertyManager.Explosive}";
            label7.Text = $"用户智慧值：{_propertyManager.Wisdom}";
            label8.Text = $"剩余时间：已废弃";
            label9.Text = $"主信号值：0";
        }

        private void Testify_Load(object sender, EventArgs e)
        {
            _accountManager = Manager.AccountManager.GetManagerInstance();
            _moneyManager = Manager.MoneyManager.GetManagerInstance();
            _propertyManager = Manager.PropertyManager.GetManagerInstance();
        }
    }
}
