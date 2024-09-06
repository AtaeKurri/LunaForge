using LunaForge.EditorData.Documents;
using ImGuiNET;
using Newtonsoft.Json;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LunaForge.EditorData.Project;

namespace LunaForge.EditorData.Nodes;

[Serializable]
public abstract class TreeNode : ICloneable
{
    #region Properties

    [JsonIgnore]
    public int Hash;

    [JsonIgnore]
    public TreeNode Parent;

    [JsonIgnore]
    public LunaDefinition ParentDef;

    [JsonProperty, DefaultValue(false)]
    public bool IsSelected { get; set; }

    [JsonProperty, DefaultValue(false)]
    public bool IsBanned { get; set; }

    [JsonProperty, DefaultValue(false)]
    public bool IsExpanded { get; set; }

    [JsonIgnore]
    public List<TreeNode> children = [];
    [JsonIgnore]
    public List<TreeNode> Children
    {
        get => children;
        set
        {
            if (value == null)
            {
                children = [];
                // Gérer l'event de changement.
            }
            else
            {
                throw new InvalidOperationException("Cannot set a Children as a null value");
            }
        }
    }

    [JsonIgnore]
    private bool IsLeafNode => Children.Count == 0;

    [JsonIgnore]
    protected bool Activated = false;
    [JsonIgnore]
    private TreeNode LinkedPrevious = null;
    [JsonIgnore]
    private TreeNode LinkedNext = null;

    [JsonIgnore]
    public string DisplayString => ToString();

    #endregion

    protected TreeNode()
    {
        
    }

    public TreeNode(LunaDefinition def)
        : this()
    {
        ParentDef = def;
        Hash = ParentDef.TreeNodeMaxHash;
        ParentDef.TreeNodeMaxHash++;
    }

    public override string ToString()
    {
        return $"You're not supposed to see that. If you do, please report to 'tania_anehina' on discord or create an issue on the git repo.\nNode: {GetType()}";
    }

    #region Rendering

    public void RenderNode()
    {
        RenderNode(this);
    }

    public void RenderNode(TreeNode node)
    {
        ImGui.PushID(Hash);

        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.FramePadding;

        if (node.IsLeafNode) 
            flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet;

        bool isExpanded = ImGui.TreeNodeEx(string.Empty, flags);
        ImGui.SameLine();
        rlImGui.ImageSize(ParentDef.ParentProject.Window.ParentWindow.FindTexture("icon"), new Vector2(16, 16));
        ImGui.SameLine();
        if (ImGui.Selectable(node.DisplayString, node.IsSelected))
        {
            DeselectAllNodes(ParentDef.TreeNodes[0]);
            node.IsSelected = true;
        }

        ImGui.PopID();

        if (isExpanded && !node.IsLeafNode)
        {
            foreach (TreeNode child in node.Children)
                RenderNode(child);
        }
    }

    private void DeselectAllNodes(TreeNode node)
    {
        node.IsSelected = false;
        foreach (TreeNode child in node.Children)
        {
            DeselectAllNodes(child);
        }
    }

    #endregion
    #region Children

    public bool ValidateChild(TreeNode nodeToValidate)
    {
        return ValidateChild(nodeToValidate, this);
    }

    public bool ValidateChild(TreeNode nodeToValidate, TreeNode sourceNode)
    {
        // TODO
        return true;
    }

    public void AddChild(TreeNode child)
    {
        Children.Add(child);
    }

    public void InsertChild(TreeNode node, int index)
    {
        Children.Insert(index, node);
    }

    public void RemoveChild(TreeNode node)
    {
        Children.Remove(node);
    }

    public void ClearChildSelection()
    {
        IsSelected = false;
        foreach (TreeNode child in Children)
            child.ClearChildSelection();
    }

    #endregion
    #region Serializer

    public void SerializeToFile(StreamWriter sw, int level)
    {
        sw.WriteLine($"{level},{TreeSerializer.SerializeTreeNode(this)}");
        foreach (TreeNode node in Children)
            node.SerializeToFile(sw, level + 1);
    }

    #endregion
    #region ToLua

    public virtual IEnumerable<string> ToLua(int spacing)
    {
        return ToLua(spacing, Children);
    }

    protected IEnumerable<string> ToLua(int spacing, IEnumerable<TreeNode> children)
    {
        foreach (TreeNode t in children)
        {
            if (!t.IsBanned)
            {
                foreach (var a in t.ToLua(spacing))
                {
                    yield return a;
                }
            }
        }
    }

    #endregion
    #region Data Handle

    public abstract IEnumerable<Tuple<int, TreeNode>> GetLines();

    protected IEnumerable<Tuple<int, TreeNode>> GetChildLines()
    {
        foreach (TreeNode node in Children)
        {
            if (node.IsBanned)
                continue;
            foreach (Tuple<int, TreeNode> tuple in node.GetLines())
            {
                yield return tuple;
            }
        }
    }

    #endregion

    public string Indent(int length) => string.Empty.PadLeft(4 * length);

    public abstract object Clone();
}
