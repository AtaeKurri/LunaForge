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

[NodeIcon("CodeSegment")]
[IgnoreValidation]
[RCInvoke(0)]
public class CodeSegment : TreeNode
{
    [JsonConstructor]
    private CodeSegment() : base() { }

    public CodeSegment(LunaDefinition def)
            : this(def, "do", "end") { }

    public CodeSegment(LunaDefinition def, string head, string tail) : base(def)
    {
        Head = head;
        Tail = tail;
    }

    [JsonIgnore, NodeAttribute("do")]
    public string Head
    {
        get => CheckAttr(0, editWindow: "code").AttrValue;
        set => CheckAttr(0, editWindow: "code").AttrValue = value;
    }

    [JsonIgnore, NodeAttribute("end")]
    public string Tail
    {
        get => CheckAttr(1, editWindow: "code").AttrValue;
        set => CheckAttr(1, editWindow: "code").AttrValue = value;
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        string s = Macrolize(0);
        int i = 1;
        foreach (char c in s)
        {
            if (c == '\n')
                i++;
        }
        yield return new Tuple<int, TreeNode>(i, this);
        foreach (Tuple<int, TreeNode> t in GetChildLines())
        {
            yield return t;
        }
        s = Macrolize(1);
        i = 1;
        foreach (char c in s)
        {
            if (c == '\n')
                i++;
        }
        yield return new Tuple<int, TreeNode>(i, this);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        Regex r = new("\\n\\b");
        string sp = Indent(spacing);
        yield return sp + r.Replace(Macrolize(0), "\n" + sp) + "\n";
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
        yield return sp + r.Replace(Macrolize(1), "\n" + sp) + "\n";
    }

    public override string ToString()
    {
        return $"{NonMacrolize(0)}\n...";
    }

    public override object Clone()
    {
        CodeSegment n = new(ParentDef);
        n.CopyData(this);
        return n;
    }
}

