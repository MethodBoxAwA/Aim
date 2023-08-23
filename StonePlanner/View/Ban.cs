using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace StonePlanner.View
{
    /// <summary>
    /// ban window
    /// </summary>
    public partial class Ban : Form
    {
        /// <summary>
        /// SendMessage function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// ReleaseCapture function
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReleaseCapture();

        /// <summary>
        /// initialize component 
        /// </summary>
        public Ban()
        {
            InitializeComponent();
        }

        /// <summary>
        /// set default value
        /// </summary>
        private void Ban_Load(object sender, EventArgs e)
        {
            //get host information
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            //get information
            string myip = IpEntry.AddressList[0].ToString();
            label_Ban.Text = $"您（{myip}）已被MBOX封禁";
            //get random code
            (int A,int B,int C,int D) = 
               (new Random().Next(1000, 9999),
               new Random().Next(1000, 9999),
               new Random().Next(1000, 9999),
               new Random().Next(1000, 9999));
            label_C.Text = $"MBAN:{A:F1}-{B:F1}-{C:F1}-{D:F1}";
        }

        /// <summary>
        /// move window
        /// </summary>
        private void panel_Capital_MouseDown(object sender, MouseEventArgs e)
        {
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int HTCAPTION = 2;

            if (e.Button == MouseButtons.Left) 
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, (IntPtr) HTCAPTION, IntPtr.Zero);// 拖动窗体  
            }
        }

        /// <summary>
        /// get local using IP address
        /// </summary>
        /// <returns>IP address</returns>
        public static string GetLocalIP()
        {
            string result = RunApp("route", "print", true);
            Match m = Regex.Match(result, @"0.0.0.0\s+0.0.0.0\s+(\d+.\d+.\d+.\d+)\s+(\d+.\d+.\d+.\d+)");
            if (m.Success)
            {
                return m.Groups[2].Value;
            }
            else
            {
                try
                {
                    System.Net.Sockets.TcpClient c = new System.Net.Sockets.TcpClient();
                    c.Connect("www.baidu.com", 80);
                    string ip = ((System.Net.IPEndPoint) c.Client.LocalEndPoint).Address.ToString();
                    c.Close();
                    return ip;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the native primary DNS
        /// </summary>
        /// <returns>DNS address</returns>
        public static string GetPrimaryDNS()
        {
            string result = RunApp("nslookup", "", true);
            Match m = Regex.Match(result, @"\d+\.\d+\.\d+\.\d+");
            if (m.Success)
            {
                return m.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// run a console application and get result
        /// </summary>
        /// <param name="filename">name of the specific program</param>
        /// <param name="arguments">input arguments</param>
        /// <param name="recordLog">log content</param>
        /// <returns>the result after run</returns>
        public static string RunApp(string filename, string arguments, bool recordLog)
        {
            try
            {
                if (recordLog)
                {
                    Trace.WriteLine(filename + " " + arguments);
                }
                Process proc = new Process();
                proc.StartInfo.FileName = filename;
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.Arguments = arguments;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;
                proc.Start();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(proc.StandardOutput.BaseStream, Encoding.Default))
                {
                    /*
                     * It seems that the calling system's nslookup has not returned data or
                     * the data has not been encoded, and the program has skipped direct execution
                     * txt = sr. ReadToEnd(), causing the returned data to be empty, so sleep
                     * causes the hardware to react
                     */
                    Thread.Sleep(100);
                    /*
                     * After calling nslookup without parameters, you can continue to enter
                     * commands to continue operations, and execute directly if the process
                     * is not stopped
                     */
                    if (!proc.HasExited)
                    {
                        /*
                         * txt = sr. The ReadToEnd() program is waiting for input, and it
                         * cannot be entered, so it is directly pinched and cannot continue 
                         * running
                         */
                        proc.Kill();
                    }
                    string txt = sr.ReadToEnd();
                    sr.Close();
                    if (recordLog)
                        Trace.WriteLine(txt);
                    return txt;
                }
            }
            catch (Exception ex)
            {
                // handle errors
                Trace.WriteLine(ex);
                return ex.Message;
            }
        }
    }
}