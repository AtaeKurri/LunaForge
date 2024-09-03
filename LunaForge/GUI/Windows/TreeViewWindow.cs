using LunaForge.EditorData.Documents;
using LunaForge.GUI.Helpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Windows;

public class TreeViewWindow : ImGuiWindow
{
    public DocumentCollection Workspaces = [];
    public LunaForgeDocument? CurrentWorkspace { get => Workspaces.GetSelectedWorkspace(); }

    public TreeViewWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Nodes"))
        {
            if (ImGui.BeginTabBar("##TreeNodeWindowTabs", ImGuiTabBarFlags.AutoSelectNewTabs))
            {
                foreach (string tabId in Workspaces.Keys)
                {
                    ImGui.PushID(tabId);
                    bool shouldStayOpen = true;
                    ImGuiTabItemFlags flags = ImGuiTabItemFlags.NoPushId;
                    if (Workspaces[tabId].IsUnsaved)
                        flags |= ImGuiTabItemFlags.UnsavedDocument;

                    if (ImGui.BeginTabItem(Workspaces[tabId].DocName, ref shouldStayOpen, flags))
                    {
                        Workspaces.SelectedTab = tabId;
                        try
                        {
                            RenderWorkspace();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }

                    if (!shouldStayOpen)
                    {
                        Workspaces.Remove(tabId);
                    }

                    ImGui.PopID();
                }
                ImGui.EndTabBar();
            }
            End();
        }
    }

    public void RenderWorkspace()
    {
        EditorData.Nodes.TreeNode rootNode = CurrentWorkspace.TreeNodes[0];
        rootNode.RenderNode();
    }
}
