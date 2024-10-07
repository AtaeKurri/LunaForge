using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;
using LunaForge.Plugins.System;
using LunaForge.Plugins;

namespace LunaForge.GUI.Windows;

internal class PluginManagerWindow : ImGuiWindow
{
    public Vector2 ModalSize = new(800, 600);
    LunaPluginInfo selectedPlugin = LunaPluginInfo.Null;

    public PluginManagerWindow()
        : base(false)
    {

    }

    public override void Render()
    {
        if (ShowWindow)
        {
            ImGui.OpenPopup("Plugin Manager");
        }

        SetModalToCenter();
        if (ImGui.BeginPopupModal("Plugin Manager", ref ShowWindow, ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking))
        {
            ImGui.BeginGroup();
            {
                if (ImGui.BeginListBox("##PluginList", new Vector2(300, ImGui.GetContentRegionAvail().Y)))
                {
                    foreach (LunaPluginInfo plugin in MainWindow.Plugins.Plugins)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(plugin.IsEnabled ? ImGuiCol.Text : ImGuiCol.TextDisabled));
                        if (ImGui.Selectable($"{plugin.Plugin.Name}", selectedPlugin.Equals(plugin)))
                            selectedPlugin = plugin;
                        ImGui.PopStyleColor();
                    }
                    ImGui.EndListBox();
                }

                ImGui.SameLine();

                if (selectedPlugin.Plugin != null)
                {
                    ImGui.BeginGroup();

                    string authors = string.Join(" ; ", selectedPlugin.Plugin.Authors);
                    ImGui.TextWrapped($"{selectedPlugin.Plugin.Name} ({(selectedPlugin.IsEnabled ? "Enabled" : "Disabled")})");
                    ImGui.TextWrapped($"Author{(selectedPlugin.Plugin.Authors.Length > 1 ? "s" : "")}: {authors}");

                    ImGui.EndGroup();
                }
            }
            ImGui.EndGroup();
        }
    }

    protected void SetModalToCenter()
    {
        Vector2 renderSize = new(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        ImGui.SetNextWindowSize(ModalSize);
        ImGui.SetNextWindowPos(renderSize / 2 - (ModalSize / 2));
    }

    protected void RenderModalButtons()
    {
        // Set buttons at the bottom.
        float availableHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
        float buttonHeight = ImGui.CalcTextSize("Close").Y + ImGui.GetStyle().FramePadding.Y * 2;
        float spacing = ImGui.GetStyle().ItemSpacing.Y + 4;
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + availableHeight - buttonHeight - spacing);

        if (ImGui.Button("Close"))
        {
            ShowWindow = false;
            ImGui.CloseCurrentPopup();
        }
    }
}
