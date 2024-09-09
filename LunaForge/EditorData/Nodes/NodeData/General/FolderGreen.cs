using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData.General;

[IsFolder, NodeIcon("FolderGreen")]
public class FolderGreen : TreeNode
{
    [JsonConstructor]
    private FolderGreen() : base() { }

    public FolderGreen(LunaDefinition def) : this(def, "Folder") { }
    public FolderGreen(LunaDefinition def, string name) : base(def)
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
        FolderGreen cloned = new(ParentDef);
        cloned.CopyData(this);
        return cloned;
    }
}
