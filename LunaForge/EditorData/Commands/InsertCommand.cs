using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;

namespace LunaForge.EditorData.Commands;

public abstract class InsertCommand(TreeNode source, TreeNode toInsert) : Command
{
    protected TreeNode Source = source;
    protected TreeNode ToInsert = toInsert;
}
