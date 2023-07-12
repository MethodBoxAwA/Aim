using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using MetroFramework.Forms;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    /// <summary>
    /// manager of plugin
    /// </summary>
    public partial class PlugManager : MetroForm
    {
        /// <summary>
        /// initialize component
        /// </summary>
        public PlugManager()
        {
            InitializeComponent();
        }

        private void PlugManager_Load(object sender, EventArgs e)
        {
            //Scan all of plug-ins
            //string path = Application.StartupPath + "\\ext";
            //string[] files = Directory.GetFiles(path, "*.dll",
            //    SearchOption.AllDirectories);
            //foreach (string dllName in files)
            //{
            //    listBox_Scanned.Items.Add(dllName.Split('\\')[7]);
            //}
            LoadPlugin(Application.StartupPath + "\\ext");
        }

        private void listBox_Scanned_SelectedIndexChanged(object sender, EventArgs e)
        {
            //get dll information
            string dllName = listBox_Scanned.SelectedItem.ToString();
            string fileName = $"{Application.StartupPath}\\ext\\{dllName}";
            if (string.IsNullOrEmpty(dllName)) 
                return;
        }

        Dictionary<string,Model.PluginModel> plugins = 
            new Dictionary<string,Model.PluginModel>();

        public bool LoadPlugin(string path) 
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            if (!directory.Exists)
            {
                return false;
            }
            else
            {
                //get all of plugin
                foreach (FileInfo file in directory.GetFiles("*.dll"))
                {
                    //judge whether dll file implement interface IDev
                    try
                    {
                        //create dll assembly
                        Assembly dllAssembly = Assembly.Load(file.FullName);
                        Type[] dllTypes = dllAssembly.GetTypes();
                        //traversal all types of dll
                        foreach (var type in dllTypes)
                        {
                            if (type.GetInterface(typeof(IDev).Name) != null)
                            {
                                //valid aim plugin
                                //guide by plugin name
                                //add to listbox(display)
                                listBox_Scanned.Items.Add(file.Name);
                                //add to dict
                                plugins.Add(file.Name,new Model.PluginModel(
                                    Activator.CreateInstance(type) as IDev));
                            }
                        }
                        return true;
                    }
                    catch 
                    {
                        //continue load dll
                        continue;
                    }
                }
            }
            return true;
        }

        private void metroButton_Submit_Click(object sender, EventArgs e)
        {

        }
    }
}
