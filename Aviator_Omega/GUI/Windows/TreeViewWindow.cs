using Aviator_Omega.EditorData.Documents;
using Aviator_Omega.GUI.ImGuiHelpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.GUI.Windows;

public class TreeViewWindow : ImGuiWindow
{
    public DocumentCollection Workspaces = [];

    private string selectedTab = string.Empty;
    public AviatorDocument CurrentWorkspace;

    public TreeViewWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Nodes"))
        {
            if (ImGui.BeginTabBar("##TreeNodeWindowTabs"))
            {
                ImGui.EndTabBar();
            }
            End();
        }
    }
}
