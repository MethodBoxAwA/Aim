using System;
using System.Runtime.InteropServices;

namespace StonePlanner.Classes.DataTypes
{
    public class Structs
    {
        [Serializable]
        public struct UserStruct
        {
            public string userName;
            public int userMoney;
            public int userExplosive;
            public int userLasting;
            public int userWisdom;
        }

        [Serializable]
        public struct PlanStruct
        {
            public string Capital;
            public int Seconds;
            public string Intro;
            public double Difficulty;
            public int UDID;
            public string Parent;
            public int Lasting;
            public int Wisdom;
            public int Explosive;
        }

        [Serializable]
        public struct PlanStructA
        {
            public string Capital { get; set; }
            public int Seconds { get; set; }
            public double Difficulty { get; set; }
            public int Lasting { get; set; }
            public int Explosive { get; set; }
            public int Wisdom { get; set; }
            public long StartTime { get; set; }
            public string Intro { get; set; }
            public string Parent { get; set; }
            public Action<int> Addsignal { get => addsignal; set => addsignal=value; }
            private Action<int> addsignal;
        }

        [Serializable]
        public class PlanStructB
        {
            public string Capital { get; set; }
            public int Seconds { get; set; }
            public double Difficulty { get; set; }
            public int Lasting { get; set; }
            public int Explosive { get; set; }
            public int Wisdom { get; set; }
            public long StartTime { get; set; }
            public string Intro { get; set;}
            public Action<int> Addsignal { get => addsignal; set => addsignal=value; }
            public int UDID;
            private Action<int> addsignal;
        }

        [Serializable]
        [ComVisible(true)]
        [StructLayout(LayoutKind.Auto)]
        public class PlanStructC
        {
            public string Capital { get; set; }
            public int Seconds { get; set; }
            public double Difficulty { get; set; }
            public int Lasting { get; set; }
            public int Explosive { get; set; }
            public int Wisdom { get; set; }
            public long StartTime { get; set; }
            public string Intro { get; set; }
            public int UDID;
            public string Parent { get; set; }
            public Action<int> Addsignal { get => addsignal; set => addsignal=value; }
            private Action<int> addsignal;
        }

        [Serializable]
        public class ChatStruct
        {
            public string req_Head;
            public string Text;
        }
    }
}
