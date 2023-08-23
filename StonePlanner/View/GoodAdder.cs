using System;
using System.Data.OleDb;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace StonePlanner.View
{
    /// <summary>
    /// add good window
    /// </summary>
    public partial class GoodAdder : MetroForm
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public GoodAdder()
        {
            InitializeComponent();
        }

        /// <summary>
        /// show ide window
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            InnerIDE iDE = new InnerIDE();
            iDE.Show();
        }

        private void AddGood(object sender, EventArgs e)
        {
            try
            {
                // connect to database
                OleDbConnection conn = new OleDbConnection(
                   $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password};"
                   ); //Jet OLEDB:Database Password=
                OleDbCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO Goods (GoodPrice,GoodName,GoodPicture,GoodIntro,UseCode) VALUES(" +
                    $"{textBox_GoodPrice.Text}," +
                    $"'{textBox_GoodName.Text}'," +
                    $"'{textBox_GoodPicture.Text}'," +
                    $"'{textBox_GoodIntro.Text}'," +
                    $"'{textBox_Function.Text}')";
                // close and dispose
                conn.Open();
                cmd.ExecuteReader();
                GC.Collect();
                Dispose();
            }
            catch 
            { 
                MessageBox.Show("输入信息有误!","添加失败",MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// choose picture
        /// </summary>
        private void Buton_Cospic_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = @"png文件|*.png|jpg文件|*.jpg|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fName = openFileDialog.FileName;
                textBox_GoodPicture.Text = fName;
            }
        }
    }
}
