using ImGuiNET;
using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LunaForge.GUI.Windows;

public class FileSystemWindow : ImGuiWindow
{
    string newFolderName = string.Empty;
    string newFileName = string.Empty;

    bool NewFolderPopupOpen = false;
    bool NewFilePopupOpen = false;

    public FileSystemWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Project Files"))
        {
            // No project
            if (ParentWindow.Workspaces.Current == null)
            {
                End();
                return;
            }

            string dirPath = ParentWindow.Workspaces.Current.PathToProjectRoot;
            RenderFileTree(dirPath);

            End();
        }
    }

    public void RenderFileTree(string directoryPath)
    {
        string[] directories = Directory.GetDirectories(directoryPath);
        string[] files = Directory.GetFiles(directoryPath);

        foreach (var dir in directories)
        {
            string folderName = Path.GetFileName(dir);

            ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnDoubleClick | ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

            bool isOpen = ImGui.TreeNodeEx($"{folderName}", flags);
            TreeNodeContextMenu(dir);
            
            if (isOpen)
            {
                RenderFileTree(dir);
                ImGui.TreePop();
            }
        }

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (fileName.EndsWith(".lfp"))
                continue;
            ImGui.Selectable(fileName);
            if (ImGui.BeginPopupContextItem())
            {
                ImGui.Text(fileName);
                ImGui.Separator();

                if (ImGui.Selectable("Open file"))
                {
                    OpenFile(file);
                }

                ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGui.GetIO().KeyShift ? ImGuiCol.Text : ImGuiCol.TextDisabled));
                if (ImGui.Selectable("Delete file") && ImGui.GetIO().KeyShift)
                {
                    FileInfo fi = new(file);
                    fi.Delete();
                }
                ImGui.PopStyleColor();
                if (!ImGui.GetIO().KeyShift && ImGui.IsItemHovered())
                    ImGui.SetTooltip("Warning: This will delete the file.\nTHIS IS NOT REVERSIBLE.\nHold SHIFT to delete.");
                ImGui.EndPopup();
            }
            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(0))
            {
                OpenFile(file);
            }
        }
    }

    public void TreeNodeContextMenu(string folderPath)
    {
        if (ImGui.BeginPopupContextItem())
        {
            ImGui.Text(Path.GetFileName(folderPath));
            ImGui.Separator();

            if (ImGui.BeginMenu("New Folder"))
            {
                ImGui.Text("Enter folder name:");
                if (ImGui.InputText("##newFolderName", ref newFolderName, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    Directory.CreateDirectory(Path.Combine(folderPath, newFolderName));
                    newFolderName = string.Empty;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("New File"))
            {
                ImGui.Text("Enter file name (with extension):");
                if (ImGui.InputText("##newFileName", ref newFileName, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                {
                    File.Create(Path.Combine(folderPath, newFileName));
                    newFileName = string.Empty;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndMenu();
            }
            ImGui.PushStyleColor(ImGuiCol.Text, ImGui.GetColorU32(ImGui.GetIO().KeyShift ? ImGuiCol.Text : ImGuiCol.TextDisabled));
            if (ImGui.Selectable("Delete") && ImGui.GetIO().KeyShift)
            {
                DirectoryInfo dir = new(folderPath);
                dir.Delete(true);
            }
            ImGui.PopStyleColor();
            if (!ImGui.GetIO().KeyShift && ImGui.IsItemHovered())
                ImGui.SetTooltip("Warning: this will delete the folder and its ENTIRE content.\nTHIS IS NOT REVERSIBLE.\nHold SHIFT to delete.");

            ImGui.EndPopup();
        }
    }

    public async Task OpenFile(string filePath)
    {
        if (ParentWindow.Workspaces.Current!.IsFileOpened(filePath))
            return; // File already opened, don't do anything.

        switch (Path.GetExtension(filePath))
        {
            case ".lfp":
                return;
            case ".lfd":
                ParentWindow.Workspaces.Current!.OpenDefinitionFile(filePath);
                break;
            case ".lua":
                ParentWindow.Workspaces.Current!.OpenScriptFile(filePath);
                break;
            default:
                return;
        }
    }
}
