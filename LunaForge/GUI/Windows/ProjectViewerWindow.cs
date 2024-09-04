using LunaForge.EditorData.Documents;
using LunaForge.GUI.Helpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaForge.EditorData.Project;

namespace LunaForge.GUI.Windows;

public class ProjectViewerWindow : ImGuiWindow
{
    public LunaForgeProject ParentProject;

    public ProjectViewerWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (!ShowWindow)
            return;

        ImGui.PushID(ParentProject.Hash);
        if (Begin($"{ParentProject.ProjectName}"))
        {
            if (ImGui.IsWindowFocused() && ParentProject.Parent.Current != ParentProject)
            {
                ParentProject.Parent.Current = ParentProject;
                Console.WriteLine($"Current Project: {ParentProject.ProjectName}");
            }
            ImGui.Text(ParentProject.ProjectName);
            End();
        }
        ImGui.PopID();

        if (!ShowWindow)
            CheckProjectSaveState();
    }

    public void CheckProjectSaveState()
    {
        ParentWindow.CloseProject(ParentProject);
    }
}
