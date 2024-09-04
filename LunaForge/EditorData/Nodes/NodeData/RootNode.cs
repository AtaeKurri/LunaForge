using LunaForge.EditorData.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData;

public class RootNode : TreeNode
{
    public RootNode() : base() { }
    public RootNode(LunaForgeDocument document) : base(document) { }

    public override string ToString() => "Root";

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        foreach (Tuple<int, TreeNode> item in GetChildLines())
            yield return item;
    }

    public override object Clone()
    {
        RootNode node = new(ParentDocument);
        //node.CopyData(this);
        return node;
    }
}
