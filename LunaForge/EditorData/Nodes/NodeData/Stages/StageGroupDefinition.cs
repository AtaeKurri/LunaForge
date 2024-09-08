using LunaForge.EditorData.Nodes.Attributes;
using LunaForge.EditorData.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.NodeData.Stages;

[DefinitionNode]
[CannotBeDeleted]
public class StageGroupDefinition : TreeNode
{
    public StageGroupDefinition() : base() { }
    public StageGroupDefinition(LunaDefinition def) : base(def) { }

    public override string ToString() => "Define Stage";

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        foreach (Tuple<int, TreeNode> item in GetChildLines())
            yield return item;
    }

    public override object Clone()
    {
        StageGroupDefinition node = new(ParentDef);
        //node.CopyData(this);
        return node;
    }
}
