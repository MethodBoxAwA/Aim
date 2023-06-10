using System.Collections.Generic;
using static Aim.Plugin.DataType;

namespace Aim.Plugin
{
    public class Run
    {
        public delegate T APIHandler<T,R>(object sender,R e);
        public event APIHandler<bool,MessageEventArgs> SendMessage;

        public Run(Dictionary<string, object> Callbacks) 
        {
            foreach (var item in Callbacks)
            {
                var _sendMessageFunc = new APIHandler<bool, MessageEventArgs>
                    ((APIHandler<bool, MessageEventArgs>)Callbacks["SendMessage"]);
                SendMessage = _sendMessageFunc;
            }
        }

        public void Main() 
        {
            //Entry point of plug-in
            MessageEventArgs args = new MessageEventArgs();
            args.Message = "Hello,Plugin!";
            SendMessage?.Invoke(this, args);
        }
    }
}
