using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using StonePlanner.Classes.DataTypes;

namespace StonePlanner
{
    internal static class DataType
    {
        internal enum ExceptionsLevel
        {
            Information,
            Caution,
            Warning,
            Error
        }

        internal class SignChangedEventArgs:EventArgs
        {
            public int Sign { get; set; }
        }

        internal class VersionInfo : Interfaces.IVersion
        {
            public string Version { get; set; }
            public int Number { get; set; }
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


    }
}
