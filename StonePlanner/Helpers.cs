using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace StonePlanner
{
    internal static class Helpers
    {
        internal static class TextHelper
        {
            public static string MultipleStrings(int multiple)
            {
                string once = "";
                while (true)
                {
                    once += " ";
                    multiple--;
                    if (multiple == 0) break;
                }
                return once;
            }
        }

        internal static class CmdHelper
        {
            private static string CmdPath = @"C:\Windows\System32\cmd.exe";
            /// <summary>
            /// 执行cmd命令 返回cmd窗口显示的信息
            /// 多命令请使用批处理命令连接符：
            /// <![CDATA[
            /// &:同时执行两个命令
            /// |:将上一个命令的输出,作为下一个命令的输入
            /// &&：当&&前的命令成功时,才执行&&后的命令
            /// ||：当||前的命令失败时,才执行||后的命令]]>
            /// </summary>
            ///<param name="cmd">执行的命令</param>
            public static string RunCmd(string cmd)
            {
                cmd = cmd.Trim().TrimEnd('&') + "&exit";//说明：不管命令是否成功均执行exit命令，否则当调用ReadToEnd()方法时，会处于假死状态
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = CmdPath;
                    p.StartInfo.UseShellExecute = false; //是否使用操作系统shell启动
                    p.StartInfo.RedirectStandardInput = true; //接受来自调用程序的输入信息
                    p.StartInfo.RedirectStandardOutput = true; //由调用程序获取输出信息
                    p.StartInfo.RedirectStandardError = true; //重定向标准错误输出
                    p.StartInfo.CreateNoWindow = true; //不显示程序窗口
                    p.Start();//启动程序

                    //向cmd窗口写入命令
                    p.StandardInput.WriteLine(cmd);
                    p.StandardInput.AutoFlush = true;

                    //获取cmd窗口的输出信息
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();//等待程序执行完退出进程
                    p.Close();

                    return output;
                }
            }
        }

        internal class INIHelper
        {

            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

            [DllImport("kernel32")]
            private static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString, string lpFileName);

            public static string Read(string section, string key, string def, string filePath)
            {
                StringBuilder sb = new StringBuilder(1024);
                GetPrivateProfileString(section, key, def, sb, 1024, filePath);
                return sb.ToString();
            }

            /// <summary>
            /// 写INI文件值
            /// </summary>
            /// <param name="section">欲在其中写入的节点名称</param>
            /// <param name="key">欲设置的项名</param>
            /// <param name="value">要写入的新字符串</param>
            /// <param name="filePath">INI文件完整路径</param>
            /// <returns>非零表示成功，零表示失败</returns>
            public static int Write(string section, string key, string value, string filePath)
            {
                return WritePrivateProfileString(section, key, value, filePath);
            }

            /// <summary>
            /// 删除节
            /// </summary>
            /// <param name="section">节点名</param>
            /// <param name="filePath">INI文件完整路径</param>
            /// <returns>非零表示成功，零表示失败</returns>
            public static int DeleteSection(string section, string filePath)
            {
                return Write(section, null, null, filePath);
            }

            /// <summary>
            /// 删除键的值
            /// </summary>
            /// <param name="section">节点名</param>
            /// <param name="key">键名</param>
            /// <param name="filePath">INI文件完整路径</param>
            /// <returns>非零表示成功，零表示失败</returns>
            public static int DeleteKey(string section, string key, string filePath)
            {
                return Write(section, key, null, filePath);
            }
        }

        internal static class CryproHelper
        {
            public static string GetMD5WithFilePath(string filePath)
            {
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider(); byte[] hash_byte = md5.ComputeHash(file);
                string str = System.BitConverter.ToString(hash_byte);
                str = str.Replace("-", "");
                return str;
            }
        }
    }
}
