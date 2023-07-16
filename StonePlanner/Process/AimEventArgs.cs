using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StonePlanner.Interfaces;

namespace StonePlanner.Process
{
    internal class AimEventArgs
    {
        public class SendMessageAimEventArgs:EventArgs,IAimEventArgs
        {
            public string Message { get ; set ; } 
            public DateTime SendTime { get; set; }
            public string Signature { get; set; }
        }

    }
}
