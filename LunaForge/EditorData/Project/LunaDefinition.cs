using LunaForge.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;
using ImGuiNET;
using LunaForge.API.Core;
using LunaForge.EditorData.Commands;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.ComponentModel;
using LunaForge.GUI;

namespace LunaForge.EditorData.Project;

public class LunaDefinition : LunaProjectFile
{
    public WorkTree TreeNodes { get; set; } = [];
    public int TreeNodeMaxHash { get; set; } = 0;

    public TreeNode? SelectedNode = null;

    public LunaDefinition(LunaForgeProject parentProj, string path)
    {
        ParentProject = parentProj;
        FullFilePath = path;
        FileName = Path.GetFileName(path);
    }

    public override string ToString() => FileName;

    #region Rendering

    public override void Render()
    {
        if (TreeNodes[0] == null)
            RenderRootTypeSelector();
        else
            RenderTreeView(TreeNodes[0], TreeNodes[0].IsBanned);
    }

    private void RenderRootTypeSelector()
    {
        ImGui.Text("This file is currently empty. Please choose the Definition Type to start editing:");

        if (ImGui.BeginListBox(string.Empty))
        {
            foreach (var type in NodeManager.DefinitionNodes)
            {
                if (ImGui.Selectable($"{type.Value}"))
                {
                    SelectDefinition(type.Key);
                }
            }
            ImGui.EndListBox();
        }
    }

    private void RenderTreeView(TreeNode node, bool parentBanned)
    {
        ImGui.PushID($"{node.DisplayString}_{node.Hash}");

        // Propagate color to the child if the parent is banned.
        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32((node.IsBanned || parentBanned) ? ImGuiCol.TextDisabled : ImGuiCol.Text));

        ImGuiTreeNodeFlags flags =
            ImGuiTreeNodeFlags.OpenOnArrow
            | ImGuiTreeNodeFlags.OpenOnDoubleClick
            | ImGuiTreeNodeFlags.SpanFullWidth
            | ImGuiTreeNodeFlags.FramePadding;
        if (node == SelectedNode)
            flags |= ImGuiTreeNodeFlags.Selected;
        if (node.HasNoChildren)
            flags |= ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet;

        bool isOpen = ImGui.TreeNodeEx(node.DisplayString, flags);
        ImGui.PopStyleColor();
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            SelectedNode = node;
        if (ImGui.BeginPopupContextItem($"{node.DisplayString}_{node.Hash}_context"))
        {
            SelectedNode = node;
            node.RenderNodeContext();
            ImGui.EndPopup();
        }
        if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            Console.WriteLine($"Double-clicked.");
        }

        ImGui.PopID();

        if (isOpen)
        {
            if (!node.HasNoChildren)
            {
                foreach (TreeNode child in node.Children)
                {
                    RenderTreeView(child, node.IsBanned || parentBanned);
                }
            }
            ImGui.TreePop();
        }
    }

    #endregion
    #region Serialization

    public static async Task<WorkTree> DeserializeTree(StreamReader sr, LunaDefinition def)
    {
        WorkTree tree = [];
        TreeNode root = null;
        TreeNode parent = null;
        TreeNode tempNode = null;
        int previousLevel = -1;
        int i;
        int levelGraduation;
        string nodeToDeserialize;
        char[] temp;
        try
        {
            while (!sr.EndOfStream)
            {
                temp = (await sr.ReadLineAsync()).ToCharArray();
                i = 0;
                while (temp[i] != ',') i++;
                nodeToDeserialize = new string(temp, i + 1, temp.Length - i - 1);
                if (previousLevel != -1)
                {
                    levelGraduation = Convert.ToInt32(new string(temp, 0, i)) - previousLevel;
                    if (levelGraduation <= 0)
                    {
                        for (int j = 0; j >= levelGraduation; j--)
                        {
                            parent = parent.Parent;
                        }
                    }
                    tempNode = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                    tempNode.ParentDef = def;
                    tempNode.Hash = def.TreeNodeMaxHash;
                    def.TreeNodeMaxHash++;
                    parent.AddChild(tempNode);
                    parent = tempNode;
                    previousLevel += levelGraduation;
                }
                else
                {
                    root = TreeSerializer.DeserializeTreeNode(nodeToDeserialize);
                    root.ParentDef = def;
                    root.Hash = def.TreeNodeMaxHash;
                    def.TreeNodeMaxHash++;
                    parent = root;
                    previousLevel = 0;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        tree.Add(root);
        return tree;
    }

    #endregion
    #region IO

    public override bool Save(bool saveAs = false)
    {
        bool result = false;
        Thread dialogThread = new(() =>
        {
            string path = "";
            if (string.IsNullOrEmpty(FullFilePath) || saveAs)
            {
                Microsoft.Win32.SaveFileDialog dialog = new()
                {
                    Filter = "LunaForge Definition (*.lfd)|*.lfd",
                    InitialDirectory = Path.GetDirectoryName(path),
                    FileName = saveAs ? string.Empty : FileName
                };
                do
                {
                    if (dialog.ShowDialog() == false) return;
                } while (string.IsNullOrEmpty(dialog.FileName));
                path = dialog.FileName;
                FullFilePath = path;
                FileName = path[(path.LastIndexOf('\\') + 1)..];
            }
            else path = FullFilePath;
            PushSavedCommand();
            try
            {
                using (StreamWriter sw = new(path))
                {
                    SerializeToFile(sw);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }
            result = true;
            return;
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
        dialogThread.Join();
        return result;
    }

    public void SerializeToFile(StreamWriter sw)
    {
        TreeNodes[0].SerializeToFile(sw, 0);
    }

    public static async Task<LunaDefinition> CreateFromFile(LunaForgeProject parentProject, string filePath)
    {
        LunaDefinition definition = new(parentProject, filePath);
        try
        {
            using (StreamReader sr = new(filePath, Encoding.UTF8))
            {
                definition.TreeNodes = await DeserializeTree(sr, definition);
            }
            return definition;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return default;
        }
    }

    public override void Close()
    {
        ParentProject.ProjectFiles.Remove(this);
    }

    #endregion
    #region TreeNodes

    private void SelectDefinition(Type definitionType)
    {
        try
        {
            TreeNodes[0] = (TreeNode)Activator.CreateInstance(definitionType);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public bool Insert(TreeNode node, bool doInvoke = true)
    {
        try
        {
            if (SelectedNode == null)
                return false;
            TreeNode oldSelection = SelectedNode;
            Command cmd = null;
            switch (ParentProject.Window.ParentWindow.InsertMode)
            {
                case InsertMode.Before:
                    if (SelectedNode.Parent == null || !SelectedNode.Parent.ValidateChild(node))
                        return false;
                    cmd = new InsertBeforeCommand(SelectedNode, node);
                    break;
                case InsertMode.Child:
                    if (!SelectedNode.ValidateChild(node))
                        return false;
                    cmd = new InsertChildCommand(SelectedNode, node);
                    break;
                case InsertMode.After:
                    if (SelectedNode.Parent == null || !SelectedNode.Parent.ValidateChild(node))
                        return false;
                    cmd = new InsertAfterCommand(SelectedNode, node);
                    break;
            }
            if (SelectedNode.Parent == null && ParentProject.Window.ParentWindow.InsertMode != InsertMode.Child)
                return false;
            if (AddAndExecuteCommand(cmd))
            {
                if (doInvoke)
                {
                    //node.CheckTrace(null, new PropertyChangedEventArgs(""));
                    CreateInvoke(node);
                }
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }

    public void CreateInvoke(TreeNode node)
    {
        //NodePropertiesDataGrid.CommitEdit();
        NodeAttribute attr = node.GetCreateInvoke();
        // TODO: inputwindow invoke.
    }

    #endregion
}
