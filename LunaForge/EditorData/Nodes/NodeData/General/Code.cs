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

[LeafNode, NodeIcon("Code")]
public class Code : TreeNode
{
    [JsonConstructor]
    private Code() : base() { }

    public Code(LunaDefinition def)
            : this(def, "") { }

    public Code(LunaDefinition def, string code) : base(def)
    {
        CodeContent = code;
    }

    [JsonIgnore, NodeAttribute]
    public string CodeContent
    {
        get => CheckAttr(0, "Code").AttrValue;
        set => CheckAttr(0, "Code").AttrValue = value;
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        string s = string.Empty;
        int i = 1;
        foreach (char c in s)
        {
            if (c == '\n') i++;
        }
        yield return new Tuple<int, TreeNode>(i, this);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        Regex r = new("\\n");
        string sp = Indent(spacing);
        string nsp = "\n" + sp;
        yield return sp + r.Replace(CodeContent, nsp) + "\n";
    }

    public override string ToString()
    {
        return CodeContent;
    }

    public override object Clone()
    {
        var n = new Code(ParentDef);
        n.CopyData(this);
        return n;
    }
}
