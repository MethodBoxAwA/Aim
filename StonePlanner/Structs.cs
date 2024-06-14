using System;
using System.Runtime.InteropServices;

namespace StonePlanner
{
    internal class Structs
    {

        //string lpCapital, int dwSeconds, string dwIntro, double dwDifficulty, int UDID,string lpParent, int dwLasting = 0, int dwExplosive = 0, int dwWisdom = 0
        [Serializable]
        internal struct PlanStruct
        {
            internal string Capital;
            internal int Seconds;
            internal string Intro;
            internal double Difficulty;
            internal int UDID;
            internal string Parent;
            internal int Lasting;
            internal int Wisdom;
            internal int Explosive;
        }

        [Serializable]
        internal struct UserStruct
        {
            internal string userName;
            internal int userMoney;
            internal int userExplosive;
            internal int userLasting;
            internal int userWisdom;
        }

        [Serializable]
        [ComVisible(true)]
        [StructLayout(LayoutKind.Auto)]
        public abstract partial class PlanBase
        {
            internal string capital;
            internal int seconds;
            internal double difficulty;
            internal int lasting;
            internal int explosive;
            internal int wisdom;
            internal long startTime;
        }

        [Serializable]
        public class PlanClassA : PlanBase
        {
            internal string intro;
            internal string parent;
            internal Action<int> Addsignal;
        }

        [Serializable]
        public class PlanClassB : PlanBase
        {
            internal string intro;
            internal int UDID;
            internal Action<int> Addsignal;
        }

        [Serializable]
        [ComVisible(true)]
        [StructLayout(LayoutKind.Auto)]
        public class PlanClassC : PlanBase
        {
            internal string intro;
            internal int UDID;
            internal string parent;
            internal Action<int> Addsignal;
        }

        interface IPlan
        {
            //public string ConstructData();
        }

        [Serializable]
        internal class UserPlan
        {
            /// <summary>
            /// The capital of this plan
            /// </summary>
            public string Capital { get; set; }

            /// <summary>
            /// The duration of this plan will continue
            /// </summary>
            public int Seconds { get; set; }

            /// <summary>
            /// The introduce of this plan
            /// </summary>
            public string Intro { get; set ; }

            /// <summary>
            /// A number from 1 to 10, in steps of 0.1, representing the difficulty of this plan
            /// </summary>
            public double Difficulty { get; set; }

            /// <summary>
            /// The unique identifier of this plan
            /// </summary>
            public int UDID { get; set; }

            /// <summary>
            /// The list to which this plan belongs
            /// </summary>
            public string Parent { get; set; }

            /// <summary>
            /// Lasting obtained from completing this plan
            /// </summary>
            public int Lasting { get; set; }

            /// <summary>
            /// Wisdom obtained from completing this plan
            /// </summary>
            public int Wisdom { get; set; }

            /// <summary>
            /// Explosive obtained from completing this plan
            /// </summary>
            public int Explosive { get; set; }

            /// <summary>
            /// Task start time
            /// </summary>
            public long StartTime { get; set; } = DateTime.Now.ToFileTimeUtc();

            /// <summary>
            /// AddSign delegate
            /// </summary>
            public Action<int> AddSign = null;

            /// <summary>
            /// Model applicable in plan building
            /// </summary>
            public PlanBuildMode BuildMode { get; set; } = PlanBuildMode.A;
        }

        /// <summary>
        /// 任务构建模式
        /// </summary>
        internal enum PlanBuildMode
        {
            /// <summary>
            /// 用于分配空白Plan对象
            /// </summary>
            A,

            /// <summary>
            /// 用于从数据库建立Plan对象
            /// </summary>
            B,

            /// <summary>
            /// 用于从新建页面建立Plan对象
            /// </summary>
            C
        }

        [Serializable]
        public class ChatStruct
        {
            internal string req_Head;
            internal string Text;
        }
    }
}
