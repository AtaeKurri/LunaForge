﻿using LunaForge.EditorData.Nodes;
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
using TextCopy;
using Raylib_cs;
using System.Numerics;
using rlImGui_cs;
using LunaForge.GUI.Helpers;

namespace LunaForge.EditorData.Project;

public class LunaDefinition : LunaProjectFile
{
    public WorkTree TreeNodes { get; set; } = [];
    public int TreeNodeMaxHash { get; set; } = 0;

    public TreeNode? SelectedNode = null;

    public LunaDefinition(LunaForgeProject parentProj, string path)
        : base(parentProj, path)
    {

    }

    #region Rendering

    public override void Render()
    {
        if (TreeNodes[0] == null)
            RenderRootTypeSelector();
        else
        {
            // TODO: Render the relevant node icons here. (insert mode, ...)
            RenderNodeToolbar();
            ImGui.Separator();
            RenderTreeView(TreeNodes[0], TreeNodes[0].IsBanned);
        }
    }

    private void RenderRootTypeSelector()
    {
        ImGui.Text("This file is currently empty. Please choose the Definition Type to start editing:");

        using (var listbox = ImRaii.ListBox(string.Empty))
        {
            if (listbox)
            {
                foreach (var type in NodeManager.DefinitionNodes)
                {
                    if (ImGui.Selectable($"{type.Value}"))
                    {
                        SelectDefinition(type.Key);
                    }
                }
            }
        }
    }

    private void RenderNodeToolbar()
    {
        if (ImGui.RadioButton("Insert Before", ParentProject.Parent.MainWin.InsertMode == InsertMode.Before))
        {
            ParentProject.Parent.MainWin.InsertMode = InsertMode.Before;
        }
        ImGui.SameLine();
        if (ImGui.RadioButton("Insert as Child", ParentProject.Parent.MainWin.InsertMode == InsertMode.Child))
        {
            ParentProject.Parent.MainWin.InsertMode = InsertMode.Child;
        }
        ImGui.SameLine();
        if (ImGui.RadioButton("Insert After", ParentProject.Parent.MainWin.InsertMode == InsertMode.After))
        {
            ParentProject.Parent.MainWin.InsertMode = InsertMode.After;
        }
    }

    private void RenderTreeView(TreeNode node, bool parentBanned)
    {
        if (node.IsSelected && (SelectedNode != node || SelectedNode == null))
            SelectedNode = node;

        ImGui.PushID($"{node.Hash}");

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
        if (node.IsExpanded)
            flags |= ImGuiTreeNodeFlags.DefaultOpen;

        
        rlImGui.ImageSize(ParentProject.Parent.MainWin.FindTexture(node.MetaData.Icon), 18, 18);
        ImGui.SameLine(0, 1.5f);

#if DEBUG
        bool isOpen = ImGui.TreeNodeEx($"{node.DisplayString} | Hash={node.Hash}", flags);
#else
    bool isOpen = ImGui.TreeNodeEx(node.DisplayString, flags);
#endif

        ImGui.PopStyleColor();
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            SelectNode(node);
        if (ImGui.BeginPopupContextItem($"{node.Hash}_context"))
        {
            SelectNode(node);
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
            node.IsExpanded = true;
            if (!node.HasNoChildren)
            {
                foreach (TreeNode child in node.Children.ToList())
                {
                    RenderTreeView(child, node.IsBanned || parentBanned);
                }
            }
            ImGui.TreePop();
        }
        else
        {
            node.IsExpanded = false;
        }
    }

    private void SelectNode(TreeNode node)
    {
        if (SelectedNode != null)
            SelectedNode!.IsSelected = false;
        SelectedNode = node;
        node.IsSelected = true;
    }

    public void DeselectAllNodes()
    {
        SelectedNode = null;
        TreeNodes[0].ClearChildSelection();
    }

    public void RevealNode(TreeNode node)
    {
        if (node == null)
            return;
        TreeNode temp = node.Parent;
        TreeNodes[0].ClearChildSelection();
        Stack<TreeNode> stack = [];
        while (temp != null)
        {
            stack.Push(temp);
            temp = temp.Parent;
        }
        while (stack.Count > 0)
            stack.Pop().IsExpanded = true;
        SelectedNode = node;
        node.IsSelected = true;
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
        SelectedNode = null;
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
                RevealNode(node);
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
            MessageBox.Show(ex.ToString());
            return false;
        }
    }

    public void CreateInvoke(TreeNode node)
    {
        NodeAttribute attr = node.GetCreateInvoke();
        // TODO: inputwindow invoke.
    }

    public void CutNode()
    {
        try
        {
            ClipboardService.SetText(TreeSerializer.SerializeTreeNode((TreeNode)SelectedNode.Clone()));
            TreeNode prev = SelectedNode.GetNearestEdited();
            AddAndExecuteCommand(new DeleteCommand(SelectedNode));
            if (prev != null)
                RevealNode(prev);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    public bool CutNode_CanExecute()
    {
        if (SelectedNode != null)
            return SelectedNode.CanLogicallyDelete();
        return false;
    }

    public void CopyNode()
    {
        try
        {
            ClipboardService.SetText(TreeSerializer.SerializeTreeNode((TreeNode)SelectedNode.Clone()));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    public bool CopyNode_CanExecute() => SelectedNode != null;

    public void PasteNode()
    {
        try
        {
            TreeNode node = TreeSerializer.DeserializeTreeNode(ClipboardService.GetText());
            node.ParentDef = this;
            TreeNode newNode = (TreeNode)node.Clone();
            Insert(newNode, false);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
    public bool PasteNode_CanExecute() => SelectedNode != null && !string.IsNullOrEmpty(ClipboardService.GetText());

    public override void Delete()
    {
        TreeNode? prev = SelectedNode?.GetNearestEdited();
        if (SelectedNode == null) return;
        AddAndExecuteCommand(new DeleteCommand(SelectedNode));
        if (prev != null)
            RevealNode(prev);
    }
    public override bool Delete_CanExecute()
    {
        if (SelectedNode == null)
            return false;
        return SelectedNode.CanLogicallyDelete();
    }

    #endregion
}
