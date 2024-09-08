using LunaForge.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

public sealed class NodeMeta
{
    public bool IsFolder { get; }

    public bool IsDefinition { get; }

    public bool IsLeafNode { get; }

    public bool CannotBeDeleted { get; }

    public string Icon { get; }

    public Type[] RequireParent { get; } = null;
    public Type[][] RequireAncestor { get; } = null;

    public int? CreateInvokeId { get; } = null;

    public NodeMeta(TreeNode node)
    {
        Type type = node.GetType();

        IsFolder = type.IsDefined(typeof(IsFolderAttribute), false);
        IsDefinition = type.IsDefined(typeof(DefinitionNodeAttribute), true);
        IsLeafNode = type.IsDefined(typeof(LeafNodeAttribute), true);
        CannotBeDeleted = type.IsDefined(typeof(CannotBeDeletedAttribute), false);
    }
}
