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
    public TabGeneral(NodePickerTab tab, MainWindow mainApp) : base(tab, mainApp) { }

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

        Tab.AddNode(new NodePickerItem(true));

        Tab.AddNode(new NodePickerItem("codeblock", "CodeBlock", "Code Block", new AddNode(AddNode_CodeBlock)));

        return Tab;
    }

    #region Delegates

    private void AddNode_Folder()
    {
        MainApp.Insert(new Folder(Def));
    }

    private void AddNode_FolderRed()
    {
        MainApp.Insert(new FolderRed(Def));
    }

    private void AddNode_FolderGreen()
    {
        MainApp.Insert(new FolderGreen(Def));
    }

    private void AddNode_FolderBlue()
    {
        MainApp.Insert(new FolderBlue(Def));
    }

    private void AddNode_Code()
    {
        MainApp.Insert(new Code(Def));
    }

    private void AddNode_CodeSegment()
    {
        MainApp.Insert(new CodeSegment(Def));
    }

    private void AddNode_Comment()
    {
        MainApp.Insert(new Comment(Def));
    }

    private void AddNode_IfNode()
    {
        TreeNode newIf = new IfNode(Def);
        newIf.AddChild(new IfThen(Def));
        newIf.AddChild(new IfElse(Def));
        MainApp.Insert(newIf);
    }

    private void AddNode_CodeBlock()
    {
        MainApp.Insert(new CodeBlock(Def));
    }

    #endregion
}
