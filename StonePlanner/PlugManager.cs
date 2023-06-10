﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using MetroFramework.Forms;
using Aim.Plugin;

namespace StonePlanner
{
    public partial class PlugManager : MetroForm
    {
        public PlugManager()
        {
            InitializeComponent();
        }

        private void PlugManager_Load(object sender, EventArgs e)
        {
            //Scan all of plug-ins
            string path = Application.StartupPath + "\\ext";
            string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
            foreach (string dllName in files)
            {
                listBox_Scanned.Items.Add(dllName.Split('\\')[7]);
            }
        }

        private void listBox_Scanned_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Get dll information
            string dllName = listBox_Scanned.SelectedItem.ToString();
            string fileName = $"{Application.StartupPath}\\ext\\{dllName}";
            if (string.IsNullOrEmpty(dllName)) 
                return; 
            Assembly assembly = Assembly.LoadFile(fileName);
            Type dllBaseData = assembly.GetType("Aim.Plugin.BaseData");
            if (dllBaseData == null)
            {
                //Not a valid aim plugin
                metroLabel_PluginName.Text = "插件名称：无效插件";
                metroLabel_PluginDescription.Text = "插件简介：该文件不是有效的Aim插件";
                metroLabel_PluginVersion.Text = "插件版本：0.0.0.0";
                metroLabel_PluginAuthor.Text = "插件作者：Null";
                return;
            }
            //Is a valid aim plugin
            metroLabel_PluginName.Text = 
                $"插件名称：{GetDllProperty<string>(dllBaseData,"Name")}";
            metroLabel_PluginDescription.Text = 
                $"插件简介：{GetDllProperty<string>(dllBaseData, "Description")}";
            metroLabel_PluginVersion.Text = 
                $"插件版本：{GetDllProperty<string>(dllBaseData, "Version")}";
            metroLabel_PluginAuthor.Text =
                $"插件作者：{GetDllProperty<string>(dllBaseData, "Developer")}";

            //Run
            Type dllRun = assembly.GetType("Aim.Plugin.Run");
            Dictionary<string, object> Callbacks = new Dictionary<string, object>();
            Aim.Plugin.Run.APIHandler<bool, Aim.Plugin.DataType.MessageEventArgs> hd =
                new Aim.Plugin.Run.APIHandler<bool, Aim.Plugin.DataType.MessageEventArgs> (MBox);
            Callbacks.Add("SendMessage", (object) hd);
            object[] parameters = new object[] { Callbacks };
            object runInstance = assembly.CreateInstance("Aim.Plugin.Run", 
                true, System.Reflection.BindingFlags.Default, null, parameters, null, null);
            MethodInfo run = dllRun.GetMethod("Main");
            run?.Invoke(runInstance,null);
        }

        public T GetDllProperty<T> (Type hDllType, string propertyName) 
            where T: IConvertible, IComparable<T>
        {
            //Get specified property of dll
            PropertyInfo property = hDllType.GetProperty(propertyName);
            object dllInstance = Activator.CreateInstance(hDllType);
            object result = property.GetValue(dllInstance);
            return (T) Convert.ChangeType(result, typeof(T));
        }

        internal bool MBox(object sender, Aim.Plugin.DataType.MessageEventArgs e) 
        {
            MessageBox.Show(e.Message);
            return true;
        }

    }
}