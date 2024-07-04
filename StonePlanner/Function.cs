﻿using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace StonePlanner
{
    public partial class Function : UserControl
    {
        string _ImageAddress = null;
        string _Caplital = "";
        string _Name = "";
        object _Callback = null;
        object _Sender = null;

        public Function(string imageAddress,string capital,string functionName,object callback = null,object sender = null)
        {
            InitializeComponent();

            this._ImageAddress = imageAddress;
            this._Caplital = capital;
            this._Name = functionName;

            this._Callback ??= callback;
            this._Sender ??= sender;
        }

        public Function(string lpCapital, string szListName,int nLineParents, object Callback = null)
        {
            InitializeComponent();

            this._ImageAddress = "";
            this._Caplital = lpCapital;
            this._Name = szListName;

            if (nLineParents == 1)
            {
                label_M.Left = 10;
            }
            else
            {
                label_M.Left = 20;
            }
        }

        private void Function_Load(object sender, EventArgs e)
        {
            if (_ImageAddress != "")
            {
                pictureBox_M.BackgroundImage = Image.FromFile(_ImageAddress);
            }
            pictureBox_M.BackgroundImageLayout = ImageLayout.Stretch;
            label_M.Text = _Caplital;
        }

        private void Function_MouseEnter(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.ControlLight;
        }

        private void Function_MouseLeave(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.Control;
        }

        private void Function_Click(object sender, EventArgs e)
        {
            switch (_Name)
            {
                case "AddTodo":
                    AddTodo addTodo = new AddTodo((AddTodo.PlanAddInvoke) _Callback, (Action<int>) this._Sender);
                    addTodo.Show();
                    break;
                case "Debugger":
                    TestTools testTools = new TestTools((Action<int>) _Callback);
                    testTools.Show();
                    break;
                case "Schedule":
                    ((Action<int>) _Callback)?.Invoke(10);
                    break;
                default:
                    // Need not any params in constructor
                    var assembly = Assembly.GetExecutingAssembly();
                    // Use full name to get type
                    var invokeWindow = assembly.GetType($"StonePlanner.{_Name}");
                    var windowInstance = (MetroForm) Activator.CreateInstance(invokeWindow);
                    windowInstance.Show();
                    break;
            }
        }
    }
}
