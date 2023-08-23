
using System.Data.OleDb;
using System.Windows.Forms;
using StonePlanner.View;

namespace StonePlanner.Classes.DataHandlers
{
    internal static class SQLConnect
    {
        internal static void SQLCommandExecution(string cmd,ref OleDbConnection odcConnection) 
        {
            //string strConn = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\data.mdb;Jet OLEDB:Database Password={Main.password}";
            //OleDbConnection odcConnection = new OleDbConnection(strConn); //2、打开连接 C#操作Access之按列读取mdb   
            //odcConnection.Open(); //建立SQL查询   
            if (odcConnection.State == System.Data.ConnectionState.Closed)
            {
                try
                {
                    odcConnection.Open();
                }
                catch 
                {
                    string strConn = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\data.mdb;Jet OLEDB:Database Password={Main.password}";
                    Main.odcConnection = new OleDbConnection(strConn); //2、打开连接 C#操作Access之按列读取mdb   
                    odcConnection.Open();
                }
            }
            OleDbCommand odCommand = odcConnection.CreateCommand();
            odCommand.CommandText = cmd; //建立读取 C#操作Access之按列读取mdb;
            odCommand.ExecuteNonQuery();
            //if (odcConnection.State == System.Data.ConnectionState.Open)
            //{
            //    odcConnection.Close();
            //}
        }

        internal static OleDbDataReader SQLCommandQuery(string cmd)
        {
            string strConn = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\data.mdb;Jet OLEDB:Database Password={Main.password}";
            OleDbConnection odcConnection = new OleDbConnection(strConn); //2、打开连接 C#操作Access之按列读取mdb   
            odcConnection.Open(); //建立SQL查询   
            OleDbCommand odCommand = odcConnection.CreateCommand();
            odCommand.CommandText = cmd; //建立读取 C#操作Access之按列读取mdb;
            var result = odCommand.ExecuteReader();
            return result;
        }

        internal static OleDbDataReader SQLCommandQuery(string cmd,ref OleDbConnection odcConnection)
        {
            string strConn = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\data.mdb;Jet OLEDB:Database Password={Main.password}";
            odcConnection = new OleDbConnection(strConn); //2、打开连接 C#操作Access之按列读取mdb   
            odcConnection.Open(); //建立SQL查询   
            OleDbCommand odCommand = odcConnection.CreateCommand();
            odCommand.CommandText = cmd; //建立读取 C#操作Access之按列读取mdb;
            var result = odCommand.ExecuteReader();
            return result;
        }
    }

    internal class SQLOperator
    {

        internal OleDbConnection _dbConnection;
        private SQLOperator _sqlOperator = null;
        private readonly object _threadLocker = null;

#nullable enable
        private SQLOperator(string dbPath,string? dbPassword)
        {
            string connectionString;

            if (dbPassword is null)
            {
                connectionString =
                    $@"Provider=Microsoft.Jet.OLEDB.4.0;" +
                    $"Data Source={dbPath};";
            }
            else
            {
                connectionString =
                    $@"Provider=Microsoft.Jet.OLEDB.4.0;" +
                    $"Data Source={dbPath};" +
                    $"Jet OLEDB:Database Password={dbPassword}";
            }

            this._dbConnection = new OleDbConnection(connectionString);
        }
#nullable disable

        internal SQLOperator GetPrototypeInstance(string dbPath, string? dbPassword)
        {
            if (this._sqlOperator is null)
            {
                lock (_threadLocker)
                {
                    this._sqlOperator ??= new SQLOperator(dbPath, dbPassword);
                }
            }

            var temp = (SQLOperator)this._sqlOperator.MemberwiseClone();
            return temp;
        }
    }
}


