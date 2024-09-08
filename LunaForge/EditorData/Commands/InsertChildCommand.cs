using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;

namespace LunaForge.EditorData.Commands;

public class InsertChildCommand : InsertCommand
{
    public InsertChildCommand(TreeNode source, TreeNode node)
        : base(source, node) { }

    public override void Execute()
    {
        Source.AddChild(ToInsert);
    }

    public override void Undo()
    {
        Source.RemoveChild(ToInsert);
    }
}
