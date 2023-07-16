using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodBox.General
{
    public class MessageEventArgs:EventArgs
    {
        /// <summary>
        /// message content
        /// </summary>
        public string Content { get; set; }
    }
}
