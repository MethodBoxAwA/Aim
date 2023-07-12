using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// <para>[temporarily abandoned]</para>
    /// <para>restrict users who have not obtained test codes from using new features</para>
    /// </summary>
    public partial class TestVersion : Form
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public TestVersion()
        {
            InitializeComponent();
        }

        private void TestVersion_Load(object sender, EventArgs e)
        {
            try
            {
                using (StreamReader sr = new StreamReader($"{Application.StartupPath}\\acco.txt"))
                {
                    string[] info = sr.ReadToEnd().Split(';');
                    textBox1.Text = info[0];
                    textBox2.Text = info[1];
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // test code
            //InnerIDE ide0 = new InnerIDE();
            //ide0.Show();
            //Hide();
            //return;
            // END OF TEST　CODE
            var request = (HttpWebRequest)WebRequest.Create(
               $"http://ps1mlx3uv.mghost.xyz/service/card.php?number={textBox1.Text}&password={textBox2.Text}");
            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (checkBox1.Checked)
            {
                StreamWriter sw = new StreamWriter($"{Application.StartupPath}\\acco.txt");
                sw.Write($"{textBox1.Text};{textBox2.Text}");
                sw.Close();
            }
            // have access
            if (responseString == "Success")
            {
                //Main main = new Main();
                //main.Show();
                //Hide();
                InnerIDE ide = new InnerIDE();
                ide.Show();
                Hide();
            }
            // prompt to user
            else
            {
                MessageBox.Show($"{responseString}");
            }
        }

        /// <summary>
        /// guide to website
        /// </summary>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://afdian.net/@MethodBox");
        }
    }
}
