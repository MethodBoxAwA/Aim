using MetroFramework.Forms;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace StonePlanner
{
    public partial class PlugIn : MetroForm
    {
        public PlugIn()
        {
            InitializeComponent();
        }

        private void PlugIn_Load(object sender, EventArgs e)
        {
            // Scan to build plug-in list
            string scanPath = $@"{Application.StartupPath}\PlugIn";

            foreach (var file in Directory.GetFiles(scanPath))
            {
                // Application extend
                if (file.EndsWith(".dll"))
                {
                    listBox_Plugs.Items.Add(file);
                }
            }
            
        }

        private void listBox_Plugs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Load plug details
            var assembly = Assembly.LoadFile((string)listBox_Plugs.Items[listBox_Plugs.SelectedIndex]);
            Type dllMain = null;

            foreach (var type in assembly.GetTypes())
            {
                if (type.Name.EndsWith("DllMain"))
                {
                    dllMain = type;
                }
            }

            // Build details
            var plugin = new DataType.Structs.PlugInDetails();
            plugin.PlugInName = (string)dllMain.GetField("PlugInName", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            plugin.PlugInDescription = (string) dllMain.GetField("PlugInDescription", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            plugin.PlugInAuthor = (string) dllMain.GetField("PlugInAuthor", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            plugin.PlugInFullName = (string) listBox_Plugs.Items[listBox_Plugs.SelectedIndex];

            // Display
            var plugDetails = new PlugDetails(plugin);
            panel_Details.Controls.Add(plugDetails);
        }
    }
}
