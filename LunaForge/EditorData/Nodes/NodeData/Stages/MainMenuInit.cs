using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using LunaForge.EditorData.Traces;
using LunaForge.EditorData.Traces.EditorTraces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData.Stages;

[NodeIcon("init")]
[CannotBeBanned, CannotBeDeleted]
[RequireParent(typeof(MainMenuDefinition))]
public class MainMenuInit : TreeNode
{
    [JsonConstructor]
    private MainMenuInit() : base() { }

    public MainMenuInit(LunaDefinition def) : base(def) { }

    public override string ToString()
    {
        return $"Main Menu:init()";
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        yield return $"{sp}function stage_init:init()\n";
        foreach (var a in base.ToLua(spacing + 1))
            yield return a;
        yield return "end\n";
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
        foreach (Tuple<int, TreeNode> t in GetChildLines())
        {
            yield return t;
        }
        yield return new Tuple<int, TreeNode>(1, this);
    }

    public override object Clone()
    {
        MainMenuInit node = new(ParentDef);
        node.CopyData(this);
        return node;
    }
}
