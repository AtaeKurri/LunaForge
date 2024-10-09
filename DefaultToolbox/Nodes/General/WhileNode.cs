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

[NodeIcon("while")]
[RCInvoke(0)]
public class WhileNode : TreeNode
{
    [JsonConstructor]
    private WhileNode() : base() { }

    public WhileNode(LunaDefinition def)
            : this(def, "") { }

    public WhileNode(LunaDefinition def, string cond) : base(def)
    {
        Condition = cond;
    }

    [JsonIgnore, NodeAttribute]
    public string Condition
    {
        get => CheckAttr(0).AttrValue;
        set => CheckAttr(0).AttrValue = value;
    }

    public IEnumerable<string> BaseToLua(int spacing, IEnumerable<TreeNode> children)
    {
        return base.ToLua(spacing, children);
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
        foreach (var a in GetChildLines())
        {
            yield return a;
        }
        yield return new Tuple<int, TreeNode>(1, this);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        var i = GetRealChildren().OrderBy((s) => (s as IIfChild)?.Priority ?? 0);
        List<TreeNode> t = new(i);

        yield return $"{sp}while {Macrolize(0)} do\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
        yield return $"{sp}end\n";
    }

    public override string ToString()
    {
        return $"While ({NonMacrolize(0)})";
    }

    public override object Clone()
    {
        WhileNode n = new(ParentDef);
        n.CopyData(this);
        return n;
    }
}
