using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.EditorData.Project;
using LunaForge.EditorData.Toolbox;
using LunaForge.GUI.Helpers;
using rlImGui_cs;

namespace LunaForge.GUI.Windows;

public class ToolboxWindow : ImGuiWindow
{
    public NodePicker NodePickerBox { get; private set; }

    public ToolboxWindow(MainWindow parent)
        : base(parent, true)
    {
        NodePickerBox = new(parent);
    }

    public override void Render()
    {
        if (BeginNoClose("Toolbox"))
        {
            if (ImGui.BeginTabBar("NodeToolbox"))
            {
                foreach (NodePickerTab tab in NodePickerBox.NodePickerTabs)
                {
                    ImGui.PushID(tab.Header);
                    if (ImGui.BeginTabItem(tab.Header))
                    {
                        ImGui.BeginDisabled(ParentWindow.Workspaces.Current?.CurrentProjectFile is not LunaDefinition);
                        //ImGui.Columns(tab.Items.Count(x => x.IsSeparator) + 1); // Number of separators.

                        foreach (NodePickerItem item in tab.Items)
                        {
                            // TODO: Fix the Hash not being set properly when read from a file.
                            if (item.IsSeparator)
                                VerticalSeparator(); //ImGui.NextColumn();
                            else
                            {
                                if (ImGui.Button(item.Tooltip))
                                    item.AddNodeMethod();
                                ImGui.SameLine();
                            }
                        }

                        ImGui.EndDisabled();
                        //ImGui.Columns(1);
                        ImGui.EndTabItem();
                    }
                    ImGui.PopID();
                }
                ImGui.EndTabBar();
            }
            End();
        }
    }

    public void RenderTabItem()
    {

    }
}
