using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Windows.Forms;
using StonePlanner.Control;

namespace StonePlanner.View
{
    /// <summary>
    /// shop
    /// </summary>
    public partial class Shop : Form
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public Shop()
        {
            InitializeComponent();
        }
        /// <summary>
        /// list of all of goods
        /// </summary>
        protected List<Good> goodList;
        /// <summary>
        /// about page information
        /// </summary>
        protected int page = 1,maxPage = 1;
        /// <summary>
        /// count of goods
        /// </summary>
        internal static int PWMS_ACCOUNT;
        private void Shop_Load(object sender, EventArgs e)
        {
            // Connect to the database to get the item
            OleDbConnection conn = new OleDbConnection(
               $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password};"
               ); 
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Goods";
            conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            if (dr.HasRows)
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    dt.Columns.Add(dr.GetName(i));
                }
                dt.Rows.Clear();
            }
            while (dr.Read())
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    row[i] = dr[i];
                }
                dt.Rows.Add(row);
            }
            cmd.Dispose();
            conn.Close();
            dataGridView1.DataSource = dt;
            dataGridView1.Visible = false;

            //display goods
            goodList = new List<Good>();
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                try
                {
                    // buy mode
                    Good good = new Good
                        (
                          dataGridView1.Rows[i].Cells[2].Value.ToString(),
                          dataGridView1.Rows[i].Cells[4].Value.ToString(),
                          Image.FromFile(dataGridView1.Rows[i].Cells[3].Value.ToString()),
                          Convert.ToInt32(dataGridView1.Rows[i].Cells[1].Value),
                          Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value),
                          new IntPtr(1) 
                        );
                    goodList.Add(good);
                }
                catch
                {
                    continue;
                }
            }
            maxPage = goodList.Count / 6 + 1;
            PWMS_ACCOUNT = goodList.Count;
            label_Main.Text = $"第{page}页，共{maxPage}页";
            ShowGood(page);
        }

        /// <summary>
        /// display goods
        /// </summary>
        /// <param name="page">page index</param>
        internal void ShowGood(int page) 
        {
            try 
            {
                int p = page - 1;
                goodList[0 + 6 * p].Location = new Point(10, 20);
                panel_Main.Controls.Add(goodList[0 + 6 * p]);
                goodList[1 + 6 * p].Location = new Point(310, 20);
                panel_Main.Controls.Add(goodList[1 + 6 * p]);
                goodList[2 + 6 * p].Location = new Point(610, 20);
                panel_Main.Controls.Add(goodList[2 + 6 * p]);
                goodList[3 + 6 * p].Location = new Point(10, 160);
                panel_Main.Controls.Add(goodList[3 + 6 * p]);
                goodList[4 + 6 * p].Location = new Point(310, 160);
                panel_Main.Controls.Add(goodList[4 + 6 * p]);
                goodList[5 + 6 * p].Location = new Point(610, 160);
                panel_Main.Controls.Add(goodList[5 + 6 * p]);
            }
            catch { }
        }

        /// <summary>
        /// page backwards
        /// </summary>
        private void btn_tRight_Click(object sender, EventArgs e)
        {
            if (page == maxPage)
            {
                MessageBox.Show("已经是最后一页！");
            }
            else
            {
                panel_Main.Controls.Clear();
                page++;
                ShowGood(page);
                label_Main.Text = $"第{page}页，共{maxPage}页";
            }
        }

        /// <summary>
        /// open AddGood window
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            GoodAdder ga = new GoodAdder();
            ga.Show();
        }

        /// <summary>
        /// page forwards
        /// </summary>
        private void btn_tLeft_Click(object sender, EventArgs e)
        {
            if (page == 1)
            {
                MessageBox.Show("已经是第一页！");
            }
            else
            {
                panel_Main.Controls.Clear();
                page--;
                ShowGood(page);
                label_Main.Text = $"第{page}页，共{maxPage}页";
            }
        }
    }
}
