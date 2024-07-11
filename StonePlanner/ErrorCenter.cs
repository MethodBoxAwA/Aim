using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using MetroFramework.Forms;
using static StonePlanner.DataType.Structs;

namespace StonePlanner
{
    public partial class ErrorCenter : MetroForm
    {
        public ErrorCenter()
        {
            InitializeComponent();

            // Switch whether use error center
            if (!Program.EnableErrorCenter)
            {
                Text = "错误中心（未启用记录）";
            }
        }

        internal static void AddError(DataType.ExceptionsLevel errLevel,Exception err) 
        {
            // Error center is disabled
            if (!Program.EnableErrorCenter)
            {
                return;
            }

            // Build error lrvel
            string levelString = errLevel switch
            {
                DataType.ExceptionsLevel.Infomation => "Infomation",
                DataType.ExceptionsLevel.Caution => "Caution",
                DataType.ExceptionsLevel.Warning => "Warning",
                DataType.ExceptionsLevel.Error => "Error",
                _ => throw new Exception("不存在这样的类型")
            };

            // Build instance
            string sourceString = "Aim";
            var error = new Error();
            error.ErrorLevel = levelString;
            error.ErrorMessage = err.Message;
            error.OccurredTime = DateTime.Now.ToString();
            error.ErrorSource = sourceString;

            // Build menory
            var entity = AccessEntity.GetAccessEntityInstance();
            entity.AddElement(error, "tb_Errors",
                new System.Collections.Generic.List<string> { "ID" });
        }

        private void ErrorCenter_Load(object sender, EventArgs e)
        {
            // Construct connection
            var entity = AccessEntity.GetAccessEntityInstance();
            var elements = entity.GetElements<Error, NonMappingTable>(
                "tb_Errors", new NonMappingTable());
            var dataTable = new DataTable();
            var errorType = typeof(Error);
            var propertyList = errorType.GetProperties();

            // Read column name
            foreach (var property in propertyList) 
            {
                dataTable.Columns.Add(property.Name);
            }
            dataTable.Rows.Clear();

            // Read specific data
            foreach (var error in elements)
            {
                var dataRow = dataTable.NewRow();

                dataRow[0] = error.ID;
                dataRow[1] = error.OccurredTime;
                dataRow[2] = error.ErrorLevel;
                dataRow[3] = error.ErrorMessage;
                dataRow[4] = error.ErrorSource;

                dataTable.Rows.Add(dataRow);
            }

            // Display data
            dataGridView1.DataSource = dataTable;
        }
    }
}