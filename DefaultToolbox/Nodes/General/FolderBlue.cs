using LunaForge.EditorData.Nodes;
using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DefaultToolbox.Nodes.General;

[IsFolder, NodeIcon("FolderBlue")]
public class FolderBlue : TreeNode
{
    [JsonConstructor]
    private FolderBlue() : base() { }

    public FolderBlue(LunaDefinition def) : this(def, "Folder") { }
    public FolderBlue(LunaDefinition def, string name) : base(def)
    {
        Name = name;
    }

    [JsonIgnore, NodeAttribute("Folder")]
    public string Name
    {
        get => CheckAttr(0).AttrValue;
        set => CheckAttr(0).AttrValue = value;
    }

    public override string ToString()
    {
        return $"[{Name}]";
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        foreach (Tuple<int, TreeNode> t in GetChildLines())
            yield return t;
    }

    public override object Clone()
    {
        FolderBlue cloned = new(ParentDef);
        cloned.CopyData(this);
        return cloned;
    }
}
