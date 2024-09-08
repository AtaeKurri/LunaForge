using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Toolbox;

public class NodePickerItem
{
    public bool IsSeparator { get; set; }
    public string Tag { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Tooltip { get; set; } = "";

    public NodePicker.AddNode AddNodeMethod { get; set; }

    public NodePickerItem(bool isSeparator)
    {
        IsSeparator = isSeparator;
    }

    public NodePickerItem(string tag, string image, string tooltip, NodePicker.AddNode addNodeMethod)
        : this(false)
    {
        Tag = tag;
        Icon = $"/LunaForge;component/Images/Nodes/{(string.IsNullOrEmpty(image) ? "Unknown.png" : image)}";
        Tooltip = tooltip;
        AddNodeMethod = addNodeMethod;
    }
}
