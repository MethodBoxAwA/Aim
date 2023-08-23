﻿using System;
using System.Drawing;
using System.Windows.Forms;
using StonePlanner.View;
using Console = StonePlanner.View.Console;

namespace StonePlanner.Control
{
    public partial class Function : UserControl
    {
        string imageAddress = null;
        string caplital = "";
        string __Name__ = "";
        object Callback = null;
        object sder = null;
        public Function(string lpImageAddress,string lpCapital,string szFunctionName,object Callback = null,object sender = null)
        {
            InitializeComponent();

            this.imageAddress = lpImageAddress;
            this.caplital = lpCapital;
            this.__Name__ = szFunctionName;

            if (Callback != null)
            {
                this.Callback = Callback;
            }
            if (sender != null)
            {
                this.sder = sender;
            }
        }
        public Function(string lpCapital, string szListName,int nLineParents, object Callback = null)
        {
            InitializeComponent();

            this.imageAddress = "";
            this.caplital = lpCapital;
            this.__Name__ = szListName;

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
            if (imageAddress != "")
            {
                pictureBox_M.BackgroundImage = Image.FromFile(imageAddress);
            }
            pictureBox_M.BackgroundImageLayout = ImageLayout.Stretch;
            label_M.Text = caplital;
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
            if (__Name__[0] == '_')
            {
                if (__Name__ == "__New__")
                {
                    AddTodo at = new AddTodo((AddTodo.PlanAddInvoke)Callback,(Action<int>)sder);
                    at.Show();
                }
                else if (__Name__ == "__Export__")
                {
                    //Main.AddSign(5);
                }
                else if (__Name__ == "__Recycle__")
                {
                    Recycle rc = new Recycle();
                    rc.Show();
                }
                else if (__Name__ == "__Infomation__")
                {
                    About ab = new About();
                    ab.Show();
                }
                else if (__Name__ == "__Console__")
                {
                    Console cs = new Console();
                    cs.Show();
                }
                else if (__Name__ == "__IDE__")
                {
                    //内测
                    //TestVersion tv = new TestVersion();
                    //tv.Show();
                    InnerIDE ide = new InnerIDE();
                    ide.Show();
                }
                else if (__Name__ == "__Settings__")
                {
                    Settings st = new Settings();
                    st.Show();
                }
                else if (__Name__ == "__Shop__")
                {
                    Shop so = new Shop();
                    so.Show();
                }
                else if (__Name__ == "__Online__")                
                {
                    WebService ws = new WebService((AddTodo.PlanAddInvoke)Callback);
                    ws.Show();
                }
                else if (__Name__ == "__Debugger__")
                {
                    TestTools tt = new TestTools((Action<int>)Callback);
                    tt.Show();
                }
                else if (__Name__ == "__Schedule__")
                {
                    ((Action<int>) Callback)?.Invoke(10);
                }
            }
            else
            {
            }
        }
    }
}
