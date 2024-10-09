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

[NodeIcon("if")]
[CreateInvoke(0), RCInvoke(0)]
public class IfNode : TreeNode
{
    [JsonConstructor]
    private IfNode() : base() { }

    public IfNode(LunaDefinition def)
            : this(def, "") { }

    public IfNode(LunaDefinition def, string code) : base(def)
    {
        Condition = code;
    }

    [JsonIgnore, NodeAttribute]
    public string Condition
    {
        get => CheckAttr(0).AttrValue;
        set => CheckAttr(0).AttrValue = value;
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

    public IEnumerable<string> BaseToLua(int spacing, IEnumerable<TreeNode> children)
    {
        return base.ToLua(spacing, children);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        var i = GetRealChildren().OrderBy((s) => (s as IIfChild)?.Priority ?? 0);
        List<TreeNode> tree = new(i);

        yield return $"{sp}if {Macrolize(0)}";
        foreach (var a in BaseToLua(spacing, i))
        {
            yield return a;
        }
        yield return $"{sp}end\n";
    }

    public override string ToString()
    {
        return $"if ({NonMacrolize(0)})";
    }

    public override object Clone()
    {
        IfNode n = new(ParentDef);
        n.CopyData(this);
        return n;
    }
}
