using System;
using MetroFramework.Forms;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// about window
    /// </summary>
    public partial class About : MetroForm
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public About()
        {
            InitializeComponent();
        }

        /// <summary>
        /// show thanks window
        /// </summary>
        public void button1_Click(object sender, EventArgs e)
        {
            Thanks t = new Thanks();
            t.Show();
        }

        /// <summary>
        /// show thanks window
        /// </summary>
        public void pictureBox2_Click(object sender, EventArgs e)
        {
            Thanks t = new Thanks();
            t.Show();
        }

        /// <summary>
        /// load version information
        /// </summary>
        private void About_Load(object sender, EventArgs e)
        {
            label4.Text += $" (内部版本{BASE_DATA.VERSION_NAME})";
        }
    }
}