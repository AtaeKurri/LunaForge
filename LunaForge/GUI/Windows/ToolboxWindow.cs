using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

    public ToolboxWindow()
        : base(true)
    {
        NodePickerBox = new();
    }

    public override void Render()
    {
        if (BeginNoClose("Toolbox", flags: ImGuiWindowFlags.NoScrollbar))
        {
            if (ImGui.BeginTabBar("NodeToolbox"))
            {
                foreach (NodePickerTab tab in NodePickerBox.NodePickerTabs)
                {
                    ImGui.PushID(tab.Header);
                    if (ImGui.BeginTabItem(tab.Header))
                    {
                        ImGui.BeginDisabled(MainWindow.Workspaces.Current?.CurrentProjectFile is not LunaDefinition);
                        //ImGui.Columns(tab.Items.Count(x => x.IsSeparator) + 1); // Number of separators.

                        foreach (NodePickerItem item in tab.Items)
                        {
                            if (item.IsSeparator)
                                VerticalSeparator(); //ImGui.NextColumn();
                            else
                            {
                                if (rlImGui.ImageButtonSize(item.Tag, MainWindow.FindTexture(item.Icon), new Vector2(24, 24)))
                                    item.AddNodeMethod();
                                if (ImGui.IsItemHovered())
                                    ImGui.SetTooltip(item.Tooltip);
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
