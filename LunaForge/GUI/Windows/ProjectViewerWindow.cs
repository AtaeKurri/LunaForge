using LunaForge.EditorData.Documents;
using LunaForge.GUI.Helpers;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaForge.EditorData.Project;
using System.Numerics;
using Raylib_cs;
using System.IO;
using System.Diagnostics;

namespace LunaForge.GUI.Windows;

public class ProjectViewerWindow : ImGuiWindow
{
    public LunaForgeProject ParentProject;

    public LunaProjectFile? fileToClose = null;
    public LunaProjectFile? filePendingModal = null;

    private bool SettingsModalClosed = true;

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

    #region Project Settings

    public string TempPathToLuaSTGExecutable;
    public bool TempUseMD5Files;
    public bool TempCheckUpdatesOnStartup;

    public void RenderProjectSettings()
    {
        Vector2 modalSize = new Vector2(700, 500);
        Vector2 renderSize = new Vector2(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        ImGui.SetNextWindowSize(modalSize);
        ImGui.SetNextWindowPos(renderSize/2 - (modalSize/2));
        if (ImGui.BeginPopupModal("Project Settings", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoDocking))
        {
            if (SettingsModalClosed)
                GetSettings();

            if (ImGui.BeginTabBar($"{ParentProject.ProjectName}_ProjectSettings"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    RenderLuaSTGPath();
                    RenderUseMD5();
                    RenderCheckUpdatesOnStartup();

                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Debug"))
                {
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            // Set buttons at the bottom.
            float availableHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
            float buttonHeight = ImGui.CalcTextSize("Ok").Y + ImGui.GetStyle().FramePadding.Y * 2;
            float spacing = ImGui.GetStyle().ItemSpacing.Y + 4;
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + availableHeight - buttonHeight - spacing);

            if (ImGui.Button("Ok"))
                ApplySettings();
            ImGui.SameLine();
            if (ImGui.Button("Apply"))
                ApplySettings(true);
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
                SettingsModalClosed = true;
            }

            ImGui.EndPopup();
        }
    }

    #region Draw Options

    public void RenderLuaSTGPath()
    {

        bool validPath = File.Exists(TempPathToLuaSTGExecutable) && TempPathToLuaSTGExecutable.EndsWith(".exe");
        ImGui.PushStyleColor(ImGuiCol.Text, validPath ? ImGui.GetColorU32(ImGuiCol.Text) : 0xFF0000FFu);
        ImGui.Text("Path to LuaSTG Executable");
        if (!validPath && ImGui.IsItemHovered())
            ImGui.SetTooltip("This path is invalid.\nCheck that it redirects to an executable file.");
        ImGui.PopStyleColor();
        ImGui.SameLine();
        ImGui.SetNextItemWidth(450);
        ImGui.InputText("##PathToLuaSTGExecutable", ref TempPathToLuaSTGExecutable, 1024);
        ImGui.SameLine();
        if (ImGui.Button("..."))
            PromptLuaSTGPath();

        ImGui.Text($"Target version: {GetTargetVersion()}");

        ImGui.Spacing();
        ImGui.Separator();
    }

    private string GetTargetVersion()
    {
        if (!File.Exists(TempPathToLuaSTGExecutable))
        {
            return "No LuaSTG exectuable set.";
        }
        FileVersionInfo LuaSTGExecutableInfos = FileVersionInfo.GetVersionInfo(TempPathToLuaSTGExecutable);
        return $"{LuaSTGExecutableInfos.ProductName} v{LuaSTGExecutableInfos.ProductVersion}";
    }

    private void RenderUseMD5()
    {
        ImGui.Checkbox("Use MD5 hash file check during packing project.", ref TempUseMD5Files);

        ImGui.Spacing();
        ImGui.Separator();
    }

    private void RenderCheckUpdatesOnStartup()
    {
        ImGui.Checkbox("Check for updates on startup.", ref TempCheckUpdatesOnStartup);

        ImGui.Spacing();
    }

    #endregion

    public void GetSettings()
    {
        TempPathToLuaSTGExecutable = ParentProject.PathToLuaSTGExecutable;
        TempUseMD5Files = ParentProject.UseMD5Files;
        TempCheckUpdatesOnStartup = ParentProject.CheckUpdatesOnStartup;

        SettingsModalClosed = false;
    }

    public void ApplySettings(bool quitPopup = false)
    {
        ParentProject.PathToLuaSTGExecutable = TempPathToLuaSTGExecutable;
        ParentProject.UseMD5Files = TempUseMD5Files;
        ParentProject.CheckUpdatesOnStartup = TempCheckUpdatesOnStartup;

        ParentProject.Save();

        if (quitPopup)
        {
            ImGui.CloseCurrentPopup();
            SettingsModalClosed = true;
        }
    }

    /// <summary>
    /// Let the user choose the LuaSTG Executable Path.
    /// </summary>
    public void PromptLuaSTGPath()
    {
        Thread dialogThread = new(() =>
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Multiselect = false,
                Filter = "LuaSTG Executable (*.exe)|*.exe",
                InitialDirectory = string.IsNullOrEmpty(ParentProject.PathToLuaSTGExecutable)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                : Path.GetDirectoryName(ParentProject.PathToLuaSTGExecutable) 
            };
            if (openFileDialog.ShowDialog() != true) return;
            TempPathToLuaSTGExecutable = openFileDialog.FileName;
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
        dialogThread.Join();
    }

    #endregion
}
