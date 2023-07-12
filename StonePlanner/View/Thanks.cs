using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StonePlanner
{
    /// <summary>
    /// show thanks list
    /// </summary>
    public partial class Thanks : Form
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public Thanks()
        {
            InitializeComponent();
        }

        private void Thanks_Load(object sender, EventArgs e)
        {
            // play specific music
            MCIPlayer.PlayMusic($"{Application.StartupPath}\\icon" +
                $"\\hlwav.mp3");
        }

        /// <summary>
        /// play the thanks list
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBox_Main.Top -= 2;
        }

        /// <summary>
        /// stop play music,but it seems like useless
        /// </summary>
        private void label_E_Click(object sender, EventArgs e)
        {
            MCIPlayer.StopMusic($"{Application.StartupPath}\\i" +
                $"con\\hlwav.mp3");
            DestroyHandle();
        }
    }
}
