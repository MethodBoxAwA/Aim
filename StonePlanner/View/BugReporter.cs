using System;
using MetroFramework.Forms;

namespace StonePlanner.View
{
    public partial class BugReporter : MetroForm
    {

        /// <summary>
        /// error information
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// initialize component and get error information
        /// </summary>
        /// <param name="info">error info</param>
        public BugReporter(string info)
        {
            InitializeComponent();

            Info = info;
        }

        /// <summary>
        /// window load
        /// </summary>
        private void BugReporter_Load(object sender, EventArgs e)
        {
            metroTextBox1.Multiline = true;
            metroTextBox1.Text = Info;
        }
    }
}
