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
            UpdateCurrentProject();

            if (ImGui.BeginTabBar($"{ParentProject.ProjectName}OpenFilesTab"))
            {
                if (ParentProject.ProjectFiles.Count == 0)
                {
                    if (ImGui.BeginTabItem("Empty"))
                    {
                        ImGui.Text("Select a file to open in the \"Project Files\" window to begin editing.");
                        ImGui.EndTabItem();
                    }
                }
                foreach (LunaProjectFile file in ParentProject.ProjectFiles.ToList())
                {
                    ImGui.PushID(file.Hash);

                    bool isOpened = file.IsOpened;
                    ImGuiTabItemFlags flags = ImGuiTabItemFlags.NoPushId;
                    if (file.IsUnsaved)
                        flags |= ImGuiTabItemFlags.UnsavedDocument;
                    if (ImGui.BeginTabItem(file.FileName, ref isOpened, flags))
                    {
                        file.Render();

                        ImGui.EndTabItem();
                    }
                    file.IsOpened = isOpened;

                    ImGui.PopID();

                    if (!file.IsOpened && ParentProject.ProjectFiles.Contains(file))
                        file.Close();
                }
                ImGui.EndTabBar();
            }

            End();
        }
        ImGui.PopID();

        if (!ShowWindow)
            CheckProjectSaveState();
    }

    public void UpdateCurrentProject()
    {
        if (ImGui.IsWindowFocused() && ParentProject.Parent.Current != ParentProject)
        {
            ParentProject.Parent.Current = ParentProject;
            Console.WriteLine($"Current Project: {ParentProject.ProjectName}");
        }
    }

    public void CheckProjectSaveState()
    {
        ParentWindow.CloseProject(ParentProject);
    }
}
