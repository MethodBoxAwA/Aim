using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    /// <summary>
    /// control
    /// </summary>
    public partial class Console : MetroForm
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage
        (
            IntPtr hWNd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam
        );
        
        /// <summary>
        /// initialize component
        /// </summary>
        public Console()
        {
            InitializeComponent();
        }

        /// <summary>
        /// function pointers STL
        /// </summary>
        internal Dictionary<string, Func<IAimEventArgs, object, object>> 
            commonFuncPointsList = new ();

        /// <summary>
        /// set default settings
        /// </summary>
        private void Console_Load(object sender, EventArgs e)
        {
            richTextBox_Output.ReadOnly = true;
            textBox_Pars.Visible = false;
            CheckForIllegalCrossThreadCalls = false;
        }

        /// <summary>
        /// returns the specified search character
        /// </summary>
        /// <param name="str">characters want to search for</param>
        /// <returns>the specified search character</returns>
        public static string GetLastWord(string str)
        {
            string x = str;
            Regex reg = new Regex(@"\S+[a-z]+\S*|[a-z]+\S*|\S+[a-z]*", RegexOptions.RightToLeft);
            x = reg.Match(x).Value;
            Regex reg2 = new Regex(@"\s");
            x = reg2.Replace(x, "");
            return x;
        }

        /// <summary>
        /// get all of key words
        /// </summary>
        /// <returns>all of key words</returns>
        public static List<string> AllClass()
        {
            List<string> list = new List<string>();
            list.Add("exit");
            list.Add("SLEEP");
            list.Add("SET");
            list.Add("ADD");
            list.Add("SIGN");
            list.Add("COMPILE");
            return list;
        }
        /// <summary>
        /// what's this?
        /// </summary>
        protected List<List<string>> rx = new List<List<string>>(); 
        int rw = 0;

        #region 语法解析器
        /// <summary>
        /// new syntax parser
        /// </summary>
        /// <param name="row">code row</param>
        protected void SyntaxParser(object rowing)
        {
            string row = (string)rowing;
            List<string> code = new(row.Split('\n'));
            foreach (var line in code)
            {
                AddString("Console","User",line);
                if (line == "exit")
                {
                    // exit aim
                    SendMessage(this.Handle, Develop.Sign.AM_EXIT, 
                        IntPtr.Zero, IntPtr.Zero);
                    Environment.Exit(0);
                }
            }
        }

        #endregion

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //命令执行
            if (e.KeyCode == Keys.Enter)
            {
                // use multi-thread
                new Thread(new ParameterizedThreadStart(SyntaxParser)).Start(richTextBox_Input.Text);
                ;
                SyntaxParser(richTextBox_Input.Text);
            }
            RichTextBox rich = (RichTextBox)sender;

            //语法高亮
            string s = GetLastWord(rich.Text);
            if (AllClass().IndexOf(s) > -1)
            {
                MySelect(rich, rich.SelectionStart, s, Color.CadetBlue, true);
            }

            //语法提示
            RichTextBox tb = (RichTextBox)sender;
            if (checkBox1.Checked)
            {
                //搜索ListBox是否已经被创建
                Control[] c = tb.Controls.Find("mylb", false);
                if (c.Length > 0)
                    ((ListBox)c[0]).Dispose();  //如果被创建则释放

                ListBox lb = new ListBox();
                lb.Name = "mylb";
                foreach (var item in AllClass())
                {
                    lb.Items.Add(item);
                }
                lb.Show();
                lb.TabIndex = 100;
                lb.Location = tb.GetPositionFromCharIndex(tb.SelectionStart);
                lb.Left += 10;
                tb.Controls.Add(lb);
            }
        }

        /// <summary>
        /// set select color
        /// </summary>
        /// <param name="tb">textbox</param>
        /// <param name="i">index</param>
        /// <param name="s">selected string</param>
        /// <param name="c">color</param>
        /// <param name="font">selected font</param>
        public static void MySelect(System.Windows.Forms.RichTextBox tb, int i, string s, Color c, bool font)
        {
            try
            {
                tb.Select(i - s.Length, s.Length);
                tb.SelectionColor = c;
                //以下是把光标放到原来位置，并把光标后输入的文字重置
                tb.Select(i, 0);
                tb.SelectionFont = new Font(" 宋体 ", 12, (FontStyle.Regular));
                tb.SelectionColor = Color.Black;
            }
            catch { }
        }


        private void richTextBox_Output_TextChanged(object sender, EventArgs e)
        {
            richTextBox_Output.SelectionStart = richTextBox_Output.Text.Length; //Set the current caret position at the end
            richTextBox_Output.ScrollToCaret(); //Now scroll it automatically
        }

        private void richTextBox_Input_Click(object sender, EventArgs e)
        {
            RichTextBox tb = (RichTextBox)sender;
            Control[] c = tb.Controls.Find("mylb", false);
            if (c.Length > 0)
                ((ListBox)c[0]).Dispose();
        }

        private void AddString(string parent, string calling, string info)
        {
            richTextBox_Output.Text +=
                $"\n{parent}@{calling}>{info}";
        }
    }
}
