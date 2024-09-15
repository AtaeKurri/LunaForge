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

namespace LunaForge.EditorData.Nodes.NodeData.General;

[NodeIcon("elseif")]
[RequireParent(typeof(IfNode))]
public class IfElseIf : TreeNode, IIfChild
{
    [JsonConstructor]
    public IfElseIf() : base() { }

    public IfElseIf(LunaDefinition def) : this(def, "") { }

    public IfElseIf(LunaDefinition def, string cond) : base(def)
    {
        Condition = cond;
    }

    [JsonIgnore, NodeAttribute]
    public string Condition
    {
        get => CheckAttr(0).AttrValue;
        set => CheckAttr(0).AttrValue = value;
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        yield return $"{sp}elseif {Macrolize(0)} then\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
    }

    public override string ToString()
    {
        return $"else if {NonMacrolize(0)}";
    }

    public override object Clone()
    {
        IfElseIf n = new(ParentDef);
        n.CopyData(this);
        return n;
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
    }

    [JsonIgnore]
    public int Priority
    {
        get => 0;
    }
}
