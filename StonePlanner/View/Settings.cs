using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Forms;
using Microsoft.Win32;
using StonePlanner.Classes.Helpers;

namespace StonePlanner.View
{
    /// <summary>
    /// software settings
    /// </summary>
    public partial class Settings : MetroForm
    {
        /// <summary>
        /// pack of settings
        /// </summary>
        internal List<string> packedSettings = new List<string>();

        /// <summary>
        /// initialize component and read settings
        /// </summary>
        public Settings()
        {
            // or set the read here
            InitializeSettings();
            InitializeComponent();
        }

        /// <summary>
        /// scroll page
        /// </summary>
        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            panel_Main.Top = -vScrollBar_Control.Value - 10;
        }

        /// <summary>
        /// save settings
        /// </summary>
        private void button_Save_Click(object sender, EventArgs e)
        {
            // write settings
            string path = $@"{Application.StartupPath}\settings.ini";
            // toggle settings
            string SwitchPicturesYesNo = checkBox_PictureSwitch.Checked ? "True" : "False";
            INIHolder.Write("SwitchSettings", "PictureSwitch",SwitchPicturesYesNo,path);
            INIHolder.Write("SwitchSettings", "PictureSwitchTime", textBox_PictureSwitchTime_R.Text, path);
            string SwitchSentencesYesNo = checkBox_PictureSwitch.Checked ? "True" : "False";
            INIHolder.Write("SwitchSettings", "SentenceSwitch", SwitchSentencesYesNo, path);
            INIHolder.Write("SwitchSettings", "SentenceSwitchTime", textBox_SentenceSwitchTime_R.Text, path);
            // correlation from self-prompting
            if (checkBox_StartSwitch.Checked)
            {
                RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                R_run.SetValue("AimPlanner", Application.ExecutablePath);
                R_run.Close();
                R_local.Close();
            }
            else
            {
                RegistryKey R_local = Registry.CurrentUser;//RegistryKey R_local = Registry.CurrentUser;
                RegistryKey R_run = R_local.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                R_run.DeleteValue("AimPlanner", false);
                R_run.Close();
                R_local.Close();
            }
            string AutoLoginYesNo = checkBox_LoginSwitch.Checked ? "True" : "False";
            INIHolder.Write("SwitchSettings", "SentenceSwitch", AutoLoginYesNo, path);
        }

        /// <summary>
        /// initialize settings
        /// </summary>
        protected void InitializeSettings() 
        {
            string path = $@"{Application.StartupPath}\settings.ini";
            packedSettings.Add(INIHolder.Read("SwitchSettings", "PictureSwitch", "False", path));
            packedSettings.Add(INIHolder.Read("SwitchSettings", "PictureSwitchTime", "False", path));
            packedSettings.Add(INIHolder.Read("SwitchSettings", "SentenceSwitch", "False", path));
            packedSettings.Add(INIHolder.Read("SwitchSettings", "SentenceSwitchTime", "False", path));
        }

        /// <summary>
        /// close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
