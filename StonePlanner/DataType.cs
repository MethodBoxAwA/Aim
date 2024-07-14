using System;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    internal class DataType
    {
        internal enum ExceptionsLevel
        {
            Infomation,
            Caution,
            Warning,
            Error
        }

        internal class SignChangedEventArgs:EventArgs
        {
            /// <summary>
            /// Changed sign
            /// </summary>
            public int Sign { get; set; }
        }

        internal class VersionInfo : IVersion
        {
            /// <summary>
            /// Software version full name
            /// </summary>
            public string Version { get; set; }

            /// <summary>
            /// Software version serial number
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// Address to download specific version of software
            /// </summary>
            public Uri DownloadUri { get; set; }

            public string GetVersion() 
            {
                return Version;
            }

            public bool IsNeedUpdate(int now) 
            {
                if (Number > now)
                    return true;
                return false;
            }

            public Uri GetUri() 
            { 
                return DownloadUri; 
            }
        }

        internal class Structs
        {
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
            internal class UserPlan : IDBEntity
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
                /// The status of current plan
                /// </summary>
                public string Status { get; set; }

                /// <summary>
                /// The introduce of this plan
                /// </summary>
                public string Intro { get; set; }

                /// <summary>
                /// A number from 1 to 10, in steps of 0.1, representing the difficulty of this plan
                /// </summary>
                public double Difficulty { get; set; }

                /// <summary>
                /// The unique identifier of this plan
                /// </summary>
                public int ID { get; set; }

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
                public string StartTime { get; set; } = DateTime.Now.ToFileTimeUtc().ToString();

                /// <summary>
                /// AddSign delegate
                /// </summary>
                public Action<Plan> AddSign = null;

                /// <summary>
                /// Model applicable in plan building
                /// </summary>
                public PlanBuildMode BuildMode { get; set; } = PlanBuildMode.A;

                public UserPlan(Plan plan)
                {
                    this.Capital = plan.Capital;
                    this.Seconds = plan.Seconds;
                    this.Status = plan.Status;
                    this.Intro = plan.Intro;
                    this.Difficulty = plan.Difficulty;
                    this.ID = plan.ID;
                    this.Parent = plan.Parent;
                    this.Lasting = plan.Lasting;
                    this.Wisdom = plan.Wisdom;
                    this.Explosive = plan.Explosive;
                    this.StartTime = plan.StartTime.ToString();
                    this.AddSign = plan.ShowDetailsHandler;
                }

                public UserPlan()
                {

                }
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

            public class User : IDBEntity
            {
                public int ID { get; set; }
                public string UserName { get; set; }
                public int UserMoney { get; set; }
                public int UserType { get; set; }
                public string UserPassword { get; set; }
                public string RestoreKey { get; set; }
                public int Lasting { get; set; }
                public int Explosive { get; set; }
                public int Wisdom { get; set; }
            }

            public class Error : IDBEntity
            {
                public int ID { get; set; }
                public string OccurredTime { get; set; }
                public string ErrorLevel { get; set; }
                public string ErrorMessage { get; set; }
                public string ErrorSource { get; set; }
            }

            public class TaskList : IDBEntity
            {
                public int ID { get; set; }
                public string ListName { get; set; }
            }

            public class Good : IDBEntity
            {
                public int ID { get; set; }
                public int GoodPrice { get; set; }
                public string GoodName { get; set; }
                public string GoodPicture { get; set; }
                public string GoodIntro { get; set; }
                public string UseCode { get; set; }
            }

            public class PlugInDetails
            {
                public string PlugInFullName { get; set; }
                public string PlugInName { get; set; }
                public string PlugInDescription { get; set; }
                public string PlugInAuthor { get; set; }
                public int Status { get; set; }
            }

            public class DataPlugIn : IDBEntity
            {
                public int ID { get; set; }
                public string PlugInMD5 { get; set; }
                public int Status { get; set; }
            }
        }
    }
}
