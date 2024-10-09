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

[NodeIcon("else")]
[RequireParent(typeof(IfNode)), Unique]
public class IfElse : TreeNode, IIfChild
{
    [JsonConstructor]
    private IfElse() : base() { }

    public IfElse(LunaDefinition def)
        : base(def) { }

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        yield return sp + "else\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
        foreach (Tuple<int, TreeNode> t in GetChildLines())
        {
            yield return t;
        }
    }

    public override string ToString()
    {
        return "else";
    }

    public override object Clone()
    {
        IfElse n = new(ParentDef);
        n.CopyData(this);
        return n;
    }

    [JsonIgnore]
    public int Priority => 1;
}
