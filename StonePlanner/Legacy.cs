using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// 此类内含所有已经被废弃的代码 以供查阅
    /// </summary>

    /*
     * 屎山 屎山 屎山
     * 下次写代码 不要用屁股写
     */
    internal static class Legacy
    {
        #region 主窗口【Main】废弃代码
        [Obsolete("该代码重新读取与写入占用大量内存", true)]
        /// <summary>
        /// 原主窗口关闭时执行的函数 功能是存储所有已保存的任务
        /// </summary>
        internal static void Main_ExitMemory()
        {
            List<Plan> recycle_bin = new List<Plan>();
            //存储
            string allTask = "";
            //链接数据库
            string strConn = $" Provider = Microsoft.Jet.OLEDB.4.0 ; Data Source = {Application.StartupPath}\\data.mdb;Jet OLEDB:Database Password={Main.password}";
            OleDbConnection myConn = new OleDbConnection(strConn);
            myConn.Open();
            //删库
            OleDbCommand inst = new OleDbCommand("DELETE * FROM Tasks", myConn);
            inst.ExecuteNonQuery();
            foreach (var item in recycle_bin)
            {
                string strInsert = " INSERT INTO Tasks ( TaskName , TaskIntro , TaskStatus , TaskTime , TaskDiff ) VALUES ( ";
                allTask += item.Capital + ";";
                //allTask += item.dwAim + ";";
                allTask += item.Seconds + ";";
                allTask += item.Difficulty;
                allTask += "\n";

                strInsert += "'" + item.Capital + "', '";
                strInsert += item.Intro + "', '";
                strInsert += item.Status + "', ";
                strInsert += item.Seconds + ", ";
                strInsert += item.Difficulty + ")";
                //清空原有数据
                inst = new OleDbCommand(strInsert, myConn);
                int lines = inst.ExecuteNonQuery();
                System.Console.WriteLine($"任务总数：{lines}");
            }
            myConn.Close();
            using (StreamWriter swA = new StreamWriter(Application.StartupPath + @"\TaskMemory.txt", true))
            {
                swA.Write(allTask);
            }
            allTask = null;
            //金钱
            StreamWriter sw = new StreamWriter($@"{Application.StartupPath}\temp\pFile114514.txt");
        }

        [Obsolete("该代码读取大量文件，占用大量内存", true)]
        /// <summary>
        /// 原主窗口加载函数 功能是读取所有的任务
        /// </summary>
        internal static void Main_LoadMemory()
        {
            List<Plan> recycle_bin = new List<Plan>();
            string allTask;
#pragma warning disable CS0219 // The variable 'GGS' is assigned but its value is never used
            double GGS = 0D;
#pragma warning restore CS0219 // The variable 'GGS' is assigned but its value is never used
            using (StreamReader sr = new StreamReader(Application.StartupPath + @"\TaskMemory.txt"))
            {
                allTask = sr.ReadToEnd();
            }
            string[] taskListString = allTask.Split('\n');

            taskListString = taskListString.Take(taskListString.Count() - 1).ToArray();

            //Plan数据存储集
            List<string> planCapital = new List<string>();
            List<int> planSecond = new List<int>();
            List<string> planIntro = new List<string>();
            List<double> planDifficulty = new List<double>();
            //链接数据库
            OleDbConnection conn = new OleDbConnection($"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={Application.StartupPath}\\data.mdb"); //Jet OLEDB:Database Password=
            OleDbCommand cmd = conn.CreateCommand();
            //读取数据
            cmd.CommandText = "select * from Tasks";
            conn.Open();
            OleDbDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            //分类获取
            for (int i = 0; i < dr.FieldCount; i++)
            {
                //Line0 => ID
                //Line1 => NAME
                //Line2 => INTRO
                //Line3 => STATUS
                //Line4 => TIME
                //Line5 => DIFF
            }
            foreach (var item in taskListString)
            {
                try
                {
                    string[] temp = item.Split(';');
                    double provider = double.Parse(temp[2], System.Globalization.NumberStyles.Float);
                    //挺怀念的，之前的方式
                    //Plan plan = new Plan(temp[0], Convert.ToInt32(temp[1]), "LEGACY", provider,"");
                    //GGS += provider;
                    //recycle_bin.Add(plan);
                }
                catch (Exception ex) { throw ex; }
            }
        }

        //internal unsafe void PlanAdder(Plan pValue, PlanClassD @struct)
        //{
        //    /*
        //     * 这个奇怪的函数真是令人费解
        //     * 已经传入了Plan了 您老是不会自己提提参数吗
        //     * 况且有地方有 有地方没有
        //     * 总言而之 屁用没有
        //     * 个人认为开发者脑子有病
        //     * 是的 是指我自己
        //     */

        //    //分配唯一编号
        //    int thisNumber = -1;
        //    for (int i = 0; i < 100; i++)
        //    {
        //        if (TasksDict[i] == null)
        //        {
        //            thisNumber = i;
        //            break;
        //        }
        //    }
        //    if (thisNumber == -1) { return; }
        //    pValue.Top = 36 * thisNumber;
        //    //获取结构体
        //    //PlanClassD @struct = Pointer.Box((void*)pStruct, typeof(PlanClassD)) as PlanClassD;
        //    //设置任务标题
        //    pValue.capital = @struct.lpCapital;
        //    //内置编号
        //    pValue.Lnumber = thisNumber;
        //    //添加时间
        //    pValue.dwSeconds = @struct.iSeconds;
        //    //添加难度
        //    pValue.dwDifficulty = @struct.dwDifficulty;
        //    //添加耐力值
        //    pValue.dwLasting = @struct.iLasting;
        //    //添加爆发值
        //    pValue.dwExplosive = @struct.iExplosive;
        //    //添加智慧值
        //    pValue.dwWisdom = @struct.iWisdom;
        //    //添加开始时间
        //    pValue.dtStartTime = DateTime.FromBinary(@struct.dwStart);
        //    //添加到字典
        //    TasksDict[thisNumber] = pValue;
        //    panel_M.Controls.Add(pValue);
        //}

        #region 编辑器【InnerIDE】废弃代码
        #endregion
    
    }
}
#endregion