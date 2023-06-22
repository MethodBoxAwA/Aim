using System.Collections.Generic;

namespace Aim.Plugin
{
    public class Run
    {
        public delegate T APIHandler<T>(object sender,object e);
        public event APIHandler<bool> SendMessage;

        public Run(Dictionary<string, object> Callbacks) 
        {
            foreach (var item in Callbacks)
            {
                var _sendMessageFunc = new APIHandler<bool>
                    ((APIHandler<bool>)Callbacks["SendMessage"]);
                SendMessage = _sendMessageFunc;
            }
        }

        public void Main() 
        {
            //Entry point of plug-in
            SendMessage?.Invoke(this, "Hello,Plugin!");
        }
    }
}
