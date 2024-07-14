using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using static StonePlanner.DataType.Structs;

namespace StonePlanner
{
    public partial class PlugDetails : UserControl
    {
        private readonly PlugInDetails _pluginDetails;
        private string _pluginMD5;

        internal PlugDetails(PlugInDetails details)
        {
            InitializeComponent();

            _pluginDetails = details;
        }

        private void PlugDetails_Load(object sender, EventArgs e)
        {
            label_PlugContent.Text = _pluginDetails.PlugInName;
            label_DeveloperContent.Text = _pluginDetails.PlugInAuthor;
            label_IntroduceContent.Text = _pluginDetails.PlugInDescription;

            // Get plug-in status
            var entity = AccessEntity.GetAccessEntityInstance();
            // Calculate MD5
            _pluginMD5 = Helpers.CryproHelper.GetMD5WithFilePath(_pluginDetails.PlugInFullName);
            var status = entity.GetElement<DataPlugIn, NonMappingTable>(
                new NonMappingTable(), "tb_Plugs", "PlugInMD5", _pluginMD5, true);
            if (status.Count == 0)
                entity.AddElement(new DataPlugIn { PlugInMD5 = _pluginMD5, Status = 0 }, "tb_Plugs", new System.Collections.Generic.List<string>() { "ID" });
            _pluginDetails.Status = status.Count != 0 ? status[0].Status : 0;
            label_StatusContent.Text = _pluginDetails.Status == 1 ? "当前状态：已启用" : "当前状态：已禁用";
        }

        private void metroButton_Enable_Click(object sender, EventArgs e)
        {
            if (_pluginDetails.Status == 1)
            {
                // Update status
                var entity = AccessEntity.GetAccessEntityInstance();
                entity.UpdateElement(new DataPlugIn { PlugInMD5 = _pluginMD5, Status = 0 }, new NonMappingTable(), "PlugInMD5", "tb_Plugs", new System.Collections.Generic.List<string>() { "ID" });
                _pluginDetails.Status = 0;
                label_StatusContent.Text = "当前状态：已禁用";
            }
            else
            {
                // Update status
                var entity = AccessEntity.GetAccessEntityInstance();
                entity.UpdateElement(new DataPlugIn { PlugInMD5 = _pluginMD5, Status = 1 }, new NonMappingTable(), "PlugInMD5", "tb_Plugs", new System.Collections.Generic.List<string>() { "ID" });
                _pluginDetails.Status = 1;
                label_StatusContent.Text = "当前状态：已启用";
            }
            
        }
    }
}