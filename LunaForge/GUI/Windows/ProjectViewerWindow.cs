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

    public LunaProjectFile? fileToClose = null;
    public LunaProjectFile? filePendingModal = null;

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

            if (ImGui.BeginTabBar($"{ParentProject.ProjectName}OpenFilesTab", ImGuiTabBarFlags.AutoSelectNewTabs))
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
                    ImGuiTabItemFlags flags = ImGuiTabItemFlags.NoPushId
                        | ImGuiTabItemFlags.NoReorder
                        | ImGuiTabItemFlags.NoAssumedClosure;
                    if (file.IsUnsaved)
                        flags |= ImGuiTabItemFlags.UnsavedDocument;

                    if (ImGui.BeginTabItem(file.FileName, ref isOpened, flags))
                    {
                        if (ParentProject.CurrentProjectFile != file)
                        {
                            ParentProject.CurrentProjectFile = file;
                            Console.WriteLine($"Current ProjectFile: {file.FileName}");
                        }
                        file.Render();

                        ImGui.EndTabItem();
                    }
                    file.IsOpened = isOpened;

                    ImGui.PopID();

                    if (!file.IsOpened && ParentProject.ProjectFiles.Contains(file))
                    {
                        if (file.IsUnsaved)
                        {
                            filePendingModal = file;
                            ImGui.OpenPopup("Confirm close of unsaved file");
                            file.IsOpened = true;
                        }
                        else
                        {
                            fileToClose = file;
                        }
                    }
                }

                if (ImGui.TabItemButton(IconFonts.FontAwesome6.Gear, ImGuiTabItemFlags.Trailing))
                {
                    ImGui.OpenPopup("Project Settings");
                }

                ConfirmCloseModal();
                RenderProjectSettings();

                // Close the file if confirmed
                if (fileToClose != null)
                    fileToClose.Close();

                ImGui.EndTabBar();
            }

            End();
        }
        ImGui.PopID();

        if (!ShowWindow)
            CheckProjectSaveState();
    }

    public void ConfirmCloseModal()
    {
        if (ImGui.BeginPopupModal("Confirm close of unsaved file", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking))
        {
            ImGui.Text($"The file \"{filePendingModal?.FileName}\" has unsaved changes. Do you really want to close it?");

            if (ImGui.Button("Yes"))
            {
                fileToClose = filePendingModal;
                filePendingModal = null;
                ImGui.CloseCurrentPopup();
            }
            ImGui.SameLine();
            if (ImGui.Button("No"))
            {
                filePendingModal = null;
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
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
        // TODO: check every opened files.
        ParentWindow.CloseProject(ParentProject);
    }

    public void RenderProjectSettings()
    {
        if (ImGui.BeginPopupModal("Project Settings", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking))
        {
            ImGui.Button("Ok");
            ImGui.SameLine();
            ImGui.Button("Apply");
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
                ImGui.CloseCurrentPopup();

            ImGui.EndPopup();
        }
    }
}
