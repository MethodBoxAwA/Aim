using System;

namespace StonePlanner.Classes.DataTypes
{
    internal class DateTimeBase
    {
        internal int Hrs;
        internal int Mins;
        internal int Secs;
    }

    //我认为这个类狗都能看懂，所以我tm一句注释也不写
    internal class DailyDateTime : DateTimeBase
    {
        private DailyDateTime(int hrs, int mins, int secs)
        {
            this.Hrs = hrs;
            this.Mins = mins;
            this.Secs = secs;
        }

        ~DailyDateTime()
        {
            Hrs = 0;
            Mins = 0;
            Secs = 0;
        }

        public override string ToString()
        {
            return $"{Hrs}:{Mins}:{Secs}";
        }

        public override bool Equals(object obj)
        {
            try
            {
                return this.Hrs == ((obj as DateTimeBase)!).Hrs && 
                       this.Mins == ((obj as DateTimeBase)!).Mins && 
                       this.Secs == ((obj as DateTimeBase)!).Secs;
            }
            catch 
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Hrs ^ Mins ^ Secs;
        }

        public static DailyDateTime operator +(DailyDateTime dateTimeLeft, 
            DailyDateTime dataTimeRight) 
        {
            var hh = dateTimeLeft.Hrs + dataTimeRight.Hrs;
            var mm = dateTimeLeft.Mins + dataTimeRight.Mins;
            var ss = dateTimeLeft.Secs + dataTimeRight.Secs;
            var result = FormatTimeA(hh * 3600 + mm * 60 + ss);
            return ConvertStringToDailyDateTimeA(result);
        }

        public static DailyDateTime operator -(DailyDateTime dateTimeLeft, 
            DailyDateTime dateTimeRight)
        {
            var hh = dateTimeLeft.Hrs - dateTimeRight.Hrs;
            var mm = dateTimeLeft.Mins - dateTimeRight.Mins;
            var ss = dateTimeLeft.Secs - dateTimeRight.Secs;
            var result = FormatTimeA(hh * 3600 + mm * 60 + ss);
            return ConvertStringToDailyDateTimeA(result);
        }

        public static DailyDateTime operator *(DailyDateTime dateType, int multiple)
        {
            var hh = dateType.Hrs * multiple;
            var mm = dateType.Mins * multiple;
            var ss = dateType.Secs * multiple;
            var result = FormatTimeA(hh * 3600 + mm * 60 + ss);
            return ConvertStringToDailyDateTimeA(result);
        }

        public static DailyDateTime operator /(DailyDateTime dateType, int multiple)
        {
            var hh = dateType.Hrs / multiple;
            var mm = dateType.Mins / multiple;
            var ss = dateType.Secs / multiple;
            var result = FormatTimeA(hh * 3600 + mm * 60 + ss);
            return ConvertStringToDailyDateTimeA(result);
        }

        private static string FormatTimeA(int T)
        {
            switch (T)
            {
                case < 60:
                    return T.ToString().Length switch
                    {
                        1 => $"00:00:0{T}",
                        2 => $"00:00:{T}",
                        _ => "00:00:00"
                    };
                case < 3600:
                {
                    var i = 0;
                    for (; T >= 60; i++)
                    {
                        T -= 60;
                    }
                    var mm = i.ToString().Length == 1 ? $"0{i}" : $"{i}";
                    var ss = T.ToString().Length == 1 ? $"0{T}" : $"{T}";
                    return $"00:{mm}:{ss}";
                }
                default:
                {
                    var j = 0;
                    for (; T >= 3600; j++)
                    {
                        T -= 3600;
                    }
                    var i = 0;
                    for (; T >= 60; i++)
                    {
                        T -= 60;
                    }
                    var hh = j.ToString().Length == 1 ? $"0{j}" : $"{j }";
                    var mm = i.ToString().Length == 1 ? $"0{i}" : $"{i }";
                    var ss = T.ToString().Length == 1 ? $"0{T}" : $"{T }";
                    return $"{hh}:{mm}:{ss}";
                }
            }
        }

        private static DailyDateTime ConvertStringToDailyDateTimeA(string res)
        {
            try
            {
                var hh = Convert.ToInt32(res.Split(':')[0]);
                var mm = Convert.ToInt32(res.Split(':')[1]);
                var ss = Convert.ToInt32(res.Split(':')[2]);
                var ddt = new DailyDateTime(hh, mm, ss);
                return ddt;
            }
            catch
            {
                throw new StringFormatException($"输入字符串\"{res}\"的格式不正确。");
            }
        }

        [Serializable]
        public class StringFormatException : Exception
        {
            public StringFormatException() { }
            public StringFormatException(string message) : base(message) { }
            public StringFormatException(string message, Exception inner) : base(message, inner) { }
            protected StringFormatException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

    }
}
