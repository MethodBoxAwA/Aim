using System;
using System.Windows.Forms;
using StonePlanner.Classes.Controls;

namespace StonePlanner.View
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
