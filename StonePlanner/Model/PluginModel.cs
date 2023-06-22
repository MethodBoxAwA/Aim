using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StonePlanner.Interfaces;

namespace StonePlanner.Model
{
    public class PluginModel
    {
        /// <summary>
        /// Initialize plugin
        /// </summary>
        /// <param name="_pluginInstance">Instance of plugin</param>
        public PluginModel(IDev _pluginInstance)
        {
            PluginInstance = _pluginInstance;
        }

        /// <summary>
        /// Plugin instance (inherit:IDev)
        /// </summary>
        IDev PluginInstance { get; }
        /// <summary>
        /// Name of plugin
        /// </summary>
        public string PluginName => PluginInstance?.PluginName;
        /// <summary>
        /// Author of plugin
        /// </summary>
        public string PluginAuthor => PluginInstance?.PluginAuthor;
        /// <summary>
        /// Description of plugin
        /// </summary>
        public string PluginDescription => PluginInstance?.PluginDescription;


    }
}
