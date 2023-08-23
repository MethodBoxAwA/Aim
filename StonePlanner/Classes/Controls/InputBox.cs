using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace StonePlanner.Classes.Controls
{
    #region DEFINE
    internal class InputBoxDefine
    {
        public const int MAX_LENGTH_OF_LPCAPTION = 128;
        public const int MAX_LENGTH_OF_LPTEXT = 512;
        public const int MAX_LENGTH_OF_SZVALUEBACK = 256;
    }

    #endregion
    internal struct InputBoxStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = InputBoxDefine.MAX_LENGTH_OF_LPCAPTION)]
        internal string Caption;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = InputBoxDefine.MAX_LENGTH_OF_LPTEXT)]
        internal string Text;
    }
    public partial class InputBox : Form
    {
        string _lpCaption;
        string _lpText;

        /// <summary>
        /// 用于从InputBox向IDE封送数据的委托
        /// </summary>
        /// <param name="s">欲封送的字符串</param>
        public delegate void SetNameInvokeBase(string s);
        SetNameInvokeBase _setNameInvoke;

        // 使用类全局变量作为控制
        internal InputBox(InputBoxStruct inputBoxStruct, SetNameInvokeBase setNameInvoke)
        {
            InitializeComponent();
            this._lpCaption = inputBoxStruct.Caption;
            this._lpText = inputBoxStruct.Text;
            this._setNameInvoke = setNameInvoke;
        }
        private void InputBox_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = _lpText;
            this.Text = _lpCaption;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _setNameInvoke(this.textBox1.Text);
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _setNameInvoke("0");
            Close();
        }

        //foreach (char item in textBox1.Text)
        //{
        //    szValueBack[i] = &item;
        //    i++;
        //}
    }
}
