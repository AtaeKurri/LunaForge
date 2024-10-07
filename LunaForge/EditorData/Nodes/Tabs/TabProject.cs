﻿using LunaForge.EditorData.Nodes.NodeData.Project;
using LunaForge.EditorData.Toolbox;
using LunaForge.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LunaForge.EditorData.Toolbox.NodePicker;

namespace LunaForge.EditorData.Nodes.Tabs;

public class TabProject : NodePickerRegister
{
    public TabProject(NodePickerTab tab) : base(tab) { }

    public override NodePickerTab RegisterTab()
    {
        Tab.AddNode(new NodePickerItem("loaddefinition", "LoadDef", "Load Definition", new AddNode(AddNode_LoadDefinition)));

        return Tab;
    }

    #region Delegates

    private void AddNode_LoadDefinition()
    {
        MainWindow.Insert(new LoadDefinition(Def));
    }

    #endregion
}
