using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using Color = Raylib_cs.Color;
using System.Reflection;
using Aviator_Omega.GUI.ImGuiHelpers;
using Aviator_Omega.GUI.Windows;

namespace Aviator_Omega.GUI;

public enum InsertMode
{
    Parent,
    Before,
    Child,
    After,
}

public class MainWindow
{
    public Version VersionNumber = Assembly.GetEntryAssembly().GetName().Version;

    #region Windows

    public NodeAttributeWindow NodeAttributeWin;
    public TreeViewWindow TreeViewWin;
    public DefinitionsWindow DefinitionsWin;
    public MessagesWindow MessagesWin;
    public DebugLogWindow DebugLogWin;

    #endregion
    #region Properties

    public InsertMode InsertMode { get; set; }

    #endregion

    public MainWindow()
    {
        NodeAttributeWin = new(this);
        TreeViewWin = new(this);
        DefinitionsWin = new(this);
        MessagesWin = new(this);
        DebugLogWin = new(this);
    }

    public void Initialize()
    {
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 800, $"Aviator v{VersionNumber}");
        Raylib.MaximizeWindow();
        Raylib.SetTargetFPS(60);

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        while (!Raylib.WindowShouldClose())
        {
            try
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);

                rlImGui.Begin();

                ImGui.DockSpaceOverViewport();
                RenderMenu();
                Render();

                rlImGui.End();

                Raylib.EndDrawing();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        Properties.Settings.Default.Save();

        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }

    private void Render()
    {
        NodeAttributeWin.Render();
        TreeViewWin.Render();
        DefinitionsWin.Render();
        MessagesWin.Render();
        DebugLogWin.Render();
    }

    private void RenderMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                ImGui.MenuItem("New");
                ImGui.MenuItem("Open");
                ImGui.Separator();
                ImGui.MenuItem("Save");
                ImGui.MenuItem("Save As");
                ImGui.Separator();
                ImGui.MenuItem("Close");
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Edit"))
            {
                ImGui.MenuItem("Edit");
                ImGui.Separator();
                ImGui.MenuItem("Undo");
                ImGui.MenuItem("Redo");
                ImGui.Separator();
                ImGui.MenuItem("Cut");
                ImGui.MenuItem("Copy");
                ImGui.MenuItem("Paste");
                ImGui.Separator();
                ImGui.MenuItem("Delete");
                ImGui.Separator();
                ImGui.MenuItem("Ban");
                ImGui.Separator();
                ImGui.MenuItem("Fold Tree");
                ImGui.MenuItem("Unfold Tree");
                ImGui.Separator();
                ImGui.MenuItem("Fold Region");
                ImGui.MenuItem("Unfold as region");
                ImGui.MenuItem("Go to Definition");
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Insert"))
            {
                ImGui.MenuItem("Parent");
                ImGui.MenuItem("Before");
                ImGui.MenuItem("After");
                ImGui.MenuItem("Child");
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Preset"))
            {
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Compile"))
            {
                ImGui.MenuItem("Run");
                ImGui.MenuItem("Test spell card");
                ImGui.MenuItem("Test from scene node");
                ImGui.MenuItem("Pack project");
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("View"))
            {
                ImGui.MenuItem("Object Definitions", string.Empty, ref DefinitionsWin.ShowWindow);
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Settings"))
            {
                ImGui.MenuItem("General settings");
                ImGui.MenuItem("Compiler settings");
                ImGui.MenuItem("Debug settings");
                ImGui.MenuItem("Editor settings");
                ImGui.EndMenu();
            }
            if (ImGui.BeginMenu("Help"))
            {
                ImGui.MenuItem("Check for Updates");
                ImGui.Separator();
                ImGui.MenuItem("About");
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }

    #region Configurations

    public bool DefinitionsWindowOpen
    {
        get => Properties.Settings.Default.DefinitionsWindowOpen;
        set
        { 
            Properties.Settings.Default.DefinitionsWindowOpen = value;
        }
    }

    #endregion
}
