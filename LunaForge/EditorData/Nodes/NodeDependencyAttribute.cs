using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

public class NodeDependencyAttribute : NodeAttribute
{
    [JsonConstructor]
    private NodeDependencyAttribute() { }

    public NodeDependencyAttribute(string name, TreeNode parent)
        : base(name, parent) { }

    public NodeDependencyAttribute(string name, string value = "", string editWindow = "")
        : base(name, value, editWindow) { }

    public NodeDependencyAttribute(string name, TreeNode parent, string editWindow)
        : base(name, parent, editWindow) { }

    public NodeDependencyAttribute(string name, string value, TreeNode parent)
        : base(name, value, parent) { }

    public NodeDependencyAttribute(string name, TreeNode parent, string editWindow, string value)
        : base(name, parent, editWindow, value) { }

    public override object Clone()
    {
        return new NodeDependencyAttribute(AttrName, ParentNode, EditWindow, AttrValue);
    }
}
