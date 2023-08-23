using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace StonePlanner.View
{
    /// <summary>
    /// save all of finished tasks
    /// </summary>
    public partial class Recycle : MetroForm
    {

        /// <summary>
        /// initialize component and load cols
        /// </summary>
        public Recycle()
        {
            InitializeComponent();

            LoadCol();
        }

        /// <summary>
        /// load task columns
        /// </summary>
        protected void LoadCol() 
        {
            // connect to database
            OleDbConnection conn = new OleDbConnection(
                $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password};"
                ); 
            // Jet OLEDB:Database Password=
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select top 10 * from Tasks";
            conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            // use DataTable
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
            // close and dispose
            cmd.Dispose();
            conn.Close();
            dataGridView1.DataSource = dt;
        }

        private void Recycle_Load(object sender, EventArgs e)
        {

        }
    }
}
