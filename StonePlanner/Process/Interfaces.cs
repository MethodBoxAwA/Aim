using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StonePlanner
{
    public class Interfaces
    {
        internal interface ISignal
        {
            int Value { get; set; }

            bool AddSignal(int Sign);
        }

        internal interface IVersion
        {
            string Version { get; set; }
            int Number { get; set; }
            Uri DownloadUri { get; set; }

            string GetVersion();
            Uri GetUri();
            bool IsNeedUpdate(int equNum);
        }

        public interface IDev
        {
            /// <summary>
            /// Name of plugin
            /// </summary>
            public string PluginName { get; }
            /// <summary>
            /// Author of plugin
            /// </summary>
            public string PluginAuthor { get; }
            /// <summary>
            /// Description of plugin
            /// </summary>
            public string PluginDescription { get; }

            /// <summary>
            /// Plugin entry point
            /// </summary>
            void Run();

            /// <summary>
            /// Plugin execute test method
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public string Execute(string input);
        }   
	
    }
}