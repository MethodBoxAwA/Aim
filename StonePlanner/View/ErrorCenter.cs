using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using MetroFramework.Forms;
using StonePlanner.Classes.DataHandlers;
using StonePlanner.Classes.DataTypes;

namespace StonePlanner.View
{
    public partial class ErrorCenter : MetroForm
    {
        /// <summary>
        /// initialize component and switch whether enable ErrorCenter
        /// </summary>
        public ErrorCenter()
        {
            InitializeComponent();

            if (!Program.EnableErrorCenter)
            {
                Text = "错误中心（未启用记录）";
            }
        }

        /// <summary>
        /// add error to ErrorCenter
        /// </summary>
        /// <param name="errLevel">error level</param>
        /// <param name="err">error instance</param>
        /// <exception cref="Exception"></exception>
        internal static void AddError(
            DataType.ExceptionsLevel errLevel,Exception err) 
        {
            if (!Program.EnableErrorCenter)
            {
                return;
            }
            // convert to specific string
            string levelString = errLevel switch
            {
                DataType.ExceptionsLevel.Information => "Information",
                DataType.ExceptionsLevel.Caution => "Caution",
                DataType.ExceptionsLevel.Warning => "Warning",
                DataType.ExceptionsLevel.Error => "Error",
                _ => throw new Exception("不存在这样的类型")
            };
            // get error trace
            string trace = err.StackTrace;
            string[] kb = trace.Split(' ');
            // insert into database
            string sourceString = $"Aim";
            SQLConnect.SQLCommandExecution($"INSERT INTO Errors (ErrTime,ErrLevel,ErrMessage,Source)" +
                $" VALUES ('{DateTime.Now}','{levelString}','{err.Message}','{sourceString}')",ref Main.odcConnection);
        }

        /// <summary>
        /// get errors from database
        /// </summary>
        private void ErrorCenter_Load(object sender, EventArgs e)
        {
            // connect to database
            OleDbConnection conn = new OleDbConnection(
      $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password};"
      ); //Jet OLEDB:Database Password=
            OleDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = "select * from Errors";
            conn.Open();
            //read all error data
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
        }
    }
}