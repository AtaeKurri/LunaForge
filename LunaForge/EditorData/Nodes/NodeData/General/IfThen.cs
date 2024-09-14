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

[Serializable, NodeIcon("then.png")]
[CannotBeBanned, CannotBeDeleted]
[RequireParent(typeof(IfNode)), Unique]
public class IfThen : TreeNode, IIfChild
{
    [JsonConstructor]
    private IfThen() : base() { }

    public IfThen(LunaDefinition def)
        : base(def) { }

    public override IEnumerable<string> ToLua(int spacing)
    {
        yield return " then\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        foreach (Tuple<int, TreeNode> t in GetChildLines())
        {
            yield return t;
        }
    }

    public override string ToString()
    {
        return "then";
    }

    public override object Clone()
    {
        IfThen n = new(ParentDef);
        n.CopyData(this);
        return n;
    }

    [JsonIgnore]
    public int Priority => -1;
}
