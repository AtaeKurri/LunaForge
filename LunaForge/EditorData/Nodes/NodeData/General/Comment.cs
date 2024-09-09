using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData.General;

[LeafNode, NodeIcon("Comment.png")]
public class Comment : TreeNode
{
    [JsonConstructor]
    private Comment() : base() { }
    public Comment(LunaDefinition def) : this(def, "") { }
    public Comment(LunaDefinition def, string comment) : base(def)
    {
        CodeComment = comment;
    }

    [JsonIgnore, NodeAttribute, DefaultValue("")]
    public string CodeComment
    {
        get => CheckAttr(0, "Comment").AttrValue;
        set => CheckAttr(0, "Comment").AttrValue = value;
    }

    public override string ToString()
    {
        return $"[Comment] {CodeComment}";
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

    public override IEnumerable<string> ToLua(int spacing)
    {
        string sp = Indent(spacing);
        yield return sp + $"--[[ {CodeComment} ]]\n";
    }

    public override object Clone()
    {
        Comment cloned = new(ParentDef);
        cloned.CopyData(this);
        return cloned;
    }
}
