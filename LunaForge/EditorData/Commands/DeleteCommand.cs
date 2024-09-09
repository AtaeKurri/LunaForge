using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;

namespace LunaForge.EditorData.Commands;

public class DeleteCommand : Command
{
    private readonly int index;
    private TreeNode toOperate;

    public DeleteCommand(TreeNode treeNode)
    {
        toOperate = treeNode;
        index = toOperate.Parent.Children.IndexOf(toOperate);
    }

    public override void Execute()
    {
        toOperate.Parent.RemoveChild(toOperate);
    }

    public override void Undo()
    {
        toOperate.Parent.InsertChild(toOperate, index);
    }
}
