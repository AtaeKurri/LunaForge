using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

[Serializable]
public class NodeAttribute : ICloneable
{
    [JsonProperty]
    public string AttrName { get; set; }
    [JsonProperty]
    public virtual string AttrValue { get; set; }
    [JsonProperty]
    public string EditWindow { get; set; }

    public TreeNode ParentNode { get; set; }

    public NodeAttribute() { }

    public NodeAttribute(string name, TreeNode parent)
    {
        AttrName = name;
        ParentNode = parent;
        AttrValue = "";
    }

    public NodeAttribute(string name, string value = "", string editWin = "")
        : this()
    {
        AttrName = name;
        AttrValue = value;
        EditWindow = editWin;
    }

    public NodeAttribute(string name, TreeNode parent, string editWin)
        : this(name, parent)
    {
        EditWindow = editWin;
    }

    public NodeAttribute(string name, string value, TreeNode parent)
        : this(name, parent)
    {
        AttrValue = value;
    }

    public NodeAttribute(string name, TreeNode parent, string editWin, string value)
        : this(name, value, parent)
    {
        EditWindow = editWin;
    }

    public virtual object Clone()
    {
        return new NodeAttribute(AttrName, ParentNode)
        {
            AttrValue = this.AttrValue,
            EditWindow = this.EditWindow
        };
    }
}