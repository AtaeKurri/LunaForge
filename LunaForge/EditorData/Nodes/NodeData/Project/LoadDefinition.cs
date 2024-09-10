using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using LunaForge.EditorData.Traces;
using LunaForge.EditorData.Traces.EditorTraces;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData.Project;

[LeafNode, NodeIcon("LoadDef")]
public class LoadDefinition : TreeNode
{
    [JsonConstructor]
    private LoadDefinition() : base() { }
    public LoadDefinition(LunaDefinition def) : this(def, "") { }
    public LoadDefinition(LunaDefinition def, string filePath) : base(def)
    {
        PathToDefinition = filePath;
    }

    [JsonIgnore, NodeAttribute, DefaultValue("")]
    public string PathToDefinition
    {
        get => CheckAttr(0, "Path to Definition").AttrValue;
        set => CheckAttr(0, "Path to Definition").AttrValue = value;
    }

    public override string ToString()
    {
        return $"Load Definition from \"{NonMacrolize(0)}\"";
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        yield return sp + $"Report error here. -> {Macrolize(0)}\n"; // TODO: Include lua file
    }

    public override object Clone()
    {
        LoadDefinition cloned = new(ParentDef);
        cloned.CopyData(this);
        return cloned;
    }

    public override List<EditorTrace> GetTraces()
    {
        List<EditorTrace> traces = [];
        if (string.IsNullOrEmpty(NonMacrolize(0)))
            traces.Add(new ArgNotNullTrace(this, GetAttr(0).AttrName));
        return traces;
    }
}
