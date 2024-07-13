using System.Windows.Forms;
using static StonePlanner.DataType.Structs;

namespace StonePlanner
{
    public partial class PlugDetails : UserControl
    {
        private readonly PlugInDetails _pluginDetails;

        internal PlugDetails(PlugInDetails details)
        {
            InitializeComponent();

            _pluginDetails = details;
        }

        private void PlugDetails_Load(object sender, System.EventArgs e)
        {
            label_PlugContent.Text = _pluginDetails.PlugInName;
            label_DeveloperContent.Text = _pluginDetails.PlugInAuthor;
            label_IntroduceContent.Text = _pluginDetails.PlugInDescription;
            label_StatusContent.Text = _pluginDetails.Status == 1 ? "当前状态：已启用" : "目前状态：已禁用";

        }
    }
}
