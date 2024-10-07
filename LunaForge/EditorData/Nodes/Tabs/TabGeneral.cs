using LunaForge.EditorData.Nodes.NodeData.General;
using LunaForge.EditorData.Toolbox;
using LunaForge.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunaForge.EditorData.Toolbox.NodePicker;

namespace LunaForge.EditorData.Nodes.Tabs;

public class TabGeneral : NodePickerRegister
{
    public TabGeneral(NodePickerTab tab) : base(tab) { }

    public override NodePickerTab RegisterTab()
    {
        Tab.AddNode(new NodePickerItem("folder", "Folder", "Folder", new AddNode(AddNode_Folder)));
        Tab.AddNode(new NodePickerItem("folderr", "FolderRed", "Red Folder", new AddNode(AddNode_FolderRed)));
        Tab.AddNode(new NodePickerItem("folderg", "FolderGreen", "Green Folder", new AddNode(AddNode_FolderGreen)));
        Tab.AddNode(new NodePickerItem("folderb", "FolderBlue", "Blue Folder", new AddNode(AddNode_FolderBlue)));
        Tab.AddNode(new NodePickerItem("code", "Code", "Code", new AddNode(AddNode_Code)));
        Tab.AddNode(new NodePickerItem("codeseg", "CodeSegment", "Code Segment", new AddNode(AddNode_CodeSegment)));
        Tab.AddNode(new NodePickerItem("comment", "Comment", "Comment", new AddNode(AddNode_Comment)));

        Tab.AddNode(new NodePickerItem(true));

        Tab.AddNode(new NodePickerItem("if", "if", "If", new AddNode(AddNode_IfNode)));
        Tab.AddNode(new NodePickerItem("else", "else", "Else", new AddNode(AddNode_IfElse)));
        Tab.AddNode(new NodePickerItem("elseif", "elseif", "Else If", new AddNode(AddNode_IfElseIf)));
        Tab.AddNode(new NodePickerItem("while", "while", "While", new AddNode(AddNode_WhileNode)));
        Tab.AddNode(new NodePickerItem("codeblock", "CodeBlock", "Code Block", new AddNode(AddNode_CodeBlock)));

        return Tab;
    }

    #region Delegates

    private void AddNode_Folder()
    {
        MainWindow.Insert(new Folder(Def));
    }

    private void AddNode_FolderRed()
    {
        MainWindow.Insert(new FolderRed(Def));
    }

    private void AddNode_FolderGreen()
    {
        MainWindow.Insert(new FolderGreen(Def));
    }

    private void AddNode_FolderBlue()
    {
        MainWindow.Insert(new FolderBlue(Def));
    }

    private void AddNode_Code()
    {
        MainWindow.Insert(new Code(Def));
    }

    private void AddNode_CodeSegment()
    {
        MainWindow.Insert(new CodeSegment(Def));
    }

    private void AddNode_Comment()
    {
        MainWindow.Insert(new Comment(Def));
    }

    private void AddNode_IfNode()
    {
        TreeNode newIf = new IfNode(Def);
        newIf.AddChild(new IfThen(Def));
        newIf.AddChild(new IfElse(Def));
        MainWindow.Insert(newIf);
    }

    private void AddNode_IfElse()
    {
        MainWindow.Insert(new IfElse(Def));
    }

    private void AddNode_IfElseIf()
    {
        MainWindow.Insert(new IfElseIf(Def));
    }

    private void AddNode_WhileNode()
    {
        MainWindow.Insert(new WhileNode(Def));
    }

    private void AddNode_CodeBlock()
    {
        MainWindow.Insert(new CodeBlock(Def));
    }

    #endregion
}
