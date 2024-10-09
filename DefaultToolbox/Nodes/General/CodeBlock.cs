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

[LeafNode, NodeIcon("CodeBlock")]
public class CodeBlock : TreeNode
{
    [JsonConstructor]
    private CodeBlock() : base() { }

    public CodeBlock(LunaDefinition def)
            : this(def, "") { }

    public CodeBlock(LunaDefinition def, string name) : base(def)
    {
        Name = name;
    }

    [JsonIgnore, NodeAttribute]
    public string Name
    {
        get => CheckAttr(0, "Code").AttrValue;
        set => CheckAttr(0, "Code").AttrValue = value;
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
        yield return sp + "do\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
        yield return sp + "end\n";
    }

    public override string ToString()
    {
        return NonMacrolize(0);
    }

    public override object Clone()
    {
        CodeBlock n = new(ParentDef);
        n.CopyData(this);
        return n;
    }
}
