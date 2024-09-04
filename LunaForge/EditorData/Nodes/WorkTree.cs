using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

[Serializable]
public class WorkTree : List<TreeNode>
{
    public WorkTree() : base() { }
    public WorkTree(IEnumerable<TreeNode> nodes) : base(nodes) { }
    public WorkTree(List<TreeNode> nodes) : base(nodes) { }
}
