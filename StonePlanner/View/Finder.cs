using System;
using System.Windows.Forms;
using MetroFramework.Forms;

namespace StonePlanner.View
{
    /// <summary>
    /// (to InnerIDE)
    /// </summary>
    public partial class Finder : MetroForm
    {
        // relevancy with InnerIDE

        /// <summary>
        /// IDE instance
        /// </summary>
        protected InnerIDE primaryIDE;

        /// <summary>
        /// initialize component and get IDE
        /// </summary>
        /// <param name="form">IDE instance</param>
        public Finder(InnerIDE form)
        {
            InitializeComponent();
            primaryIDE = form;
        }

        /// <summary>
        /// load window,the find down feature is enabled by default
        /// </summary>
        private void Finder_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = true;
        }

        // pointers
        int start = 0; int sun = 0; int count = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                RichTextBox rtxBox = primaryIDE.rtbMain;
                string str = this.textBox1.Text;
                // whether to enable case sensitivity
                if (this.checkBox1.Checked)
                {
                    //look down
                    this.FindDownM(rtxBox, str);
                }
                else
                {
                    if (this.radioButton2.Checked)
                    {
                        this.FindDown(rtxBox, str);
                    }
                    else
                    {
                        this.FindUp(rtxBox, str);//向上查找
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message,"出现错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Replace the text in textBox1 with the text in textBox2
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string str0 = this.textBox1.Text, str1 = this.textBox2.Text;
                this.replace(str0, str1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// replace all
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                RichTextBox rbox = primaryIDE.rtbMain;
                string str0 = this.textBox1.Text, str1 = 
                    this.textBox2.Text;
                this.ReplaceAll(rbox, str0, str1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// find specific character
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// function for lookup up
        /// </summary>
        private void FindUp(RichTextBox rtxBox, string str)
        {
            try
            {
                int rbox1 = rtxBox.SelectionStart;
                int index = rtxBox.Find(str, 0, rbox1,
                    RichTextBoxFinds.Reverse);
                if (index > -1)
                {
                    rtxBox.SelectionStart = index;
                    rtxBox.SelectionLength = str.Length;
                    sun++;
                    rtxBox.Focus();
                }
                else if (index < 0)
                {
                    seeks(str);
                    sun = 0;
                    // rtxBox.SelectionStart = rtxBox.Text.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// function for finding down
        /// </summary>
        private void FindDown(RichTextBox rtxBox, string str)
        {
            try
            {
                int rtxBoxTextLength = rtxBox.Text.Length;
                if (start < rtxBoxTextLength)
                {
                    start = rtxBox.Find(str, start, RichTextBoxFinds.None);
                    int los = rtxBox.SelectionStart + str.Length;
                    if ((start < 0) || (start > rtxBoxTextLength))
                    {
                        if (count == 0)
                        {
                            this.seeks(str);
                            start = los;
                            sun = 0;
                        }
                        else
                        {
                            this.seeks(str);
                            start = los;
                            sun = 0;
                        }
                    }
                    else if (start == rtxBoxTextLength || start < 0)
                    {
                        this.seeks(str);
                        start = los;
                        sun = 0;
                    }
                    else
                    {
                        sun++;
                        start = los;
                        rtxBox.Focus();
                    }
                }
                else if (start == rtxBoxTextLength || start < 0)
                {
                    int los = rtxBox.SelectionStart + str.Length;
                    this.seeks(str);
                    start = los;
                    sun = 0;
                }
                else
                {
                    int los = rtxBox.SelectionStart + str.Length;
                    this.seeks(str);
                    start = los;
                    sun = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// function for finding down
        /// </summary>
        private void FindDownM(RichTextBox rtxBox, string strFind)
        {
            try
            {
                int rtxBoxTextLength = rtxBox.Text.Length;
                if (start < rtxBoxTextLength)
                {
                    start = rtxBox.Find(strFind, start, RichTextBoxFinds.MatchCase);
                    int los = rtxBox.SelectionStart + strFind.Length;
                    if ((start < 0) || (start > rtxBoxTextLength))
                    {
                        if (count == 0)
                        {
                            this.seeks(strFind);
                            start = los;
                            sun = 0;
                        }
                        else
                        {
                            this.seeks(strFind);
                            start = los;
                            sun = 0;
                        }
                    }
                    else if (start == rtxBoxTextLength || start < 0)
                    {
                        this.seeks(strFind);
                        start = los;
                        sun = 0;
                    }
                    else
                    {
                        sun++;
                        start = los;
                        rtxBox.Focus();
                    }
                }
                else if (start == rtxBoxTextLength || start < 0)
                {
                    int los = rtxBox.SelectionStart + strFind.Length;
                    this.seeks(strFind);
                    start = los;
                    sun = 0;
                }
                else
                {
                    int los = rtxBox.SelectionStart + strFind.Length;
                    this.seeks(strFind);
                    start = los;
                    sun = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// pop-up window to prompt
        /// </summary>
        /// <param name="str">handled string</param>
        private void seeks(string str)
        {
            if (sun != 0)
            {
                MessageBox.Show(string.Format
                    ("查找完毕，共[{0}]个\"{1}\"!", sun, str), "查找—温馨提示");
            }
            else
            {
                MessageBox.Show(String.Format("\"{0}\"!", str), 
                    "查找—温馨提示");
            }
        }

        /// <summary>
        /// replace all <code>strPrevious</code> to <code>strNew</code>
        /// </summary>
        /// <param name="rtxBox">the RichTextBox to replace</param>
        /// <param name="strPrevious">previous string</param>
        /// <param name="strNew">new string</param>
        private void ReplaceAll(RichTextBox rtxBox, 
            string strPrevious, string strNew)
        {
            try
            {
                rtxBox.Text = rtxBox.Text.Replace(strPrevious, strNew);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// replace single <code>strPrevious</code> to <code>strNew</code>
        /// </summary>
        /// <param name="strPrevious">previous string</param>
        /// <param name="strNew">new string</param>
        private void replace(string strPrevious, string strNew)
        {
            try
            {
                RichTextBox rbox = primaryIDE.rtbMain;
                rbox.SelectionLength = strPrevious.Length;
                rbox.SelectedText = strNew;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "出现错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// text trigger
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.button1.Enabled = true;
        }
    }
}