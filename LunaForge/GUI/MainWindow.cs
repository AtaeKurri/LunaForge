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
using LunaForge.GUI.Helpers;
using LunaForge.GUI.Windows;
using System.IO;
using Newtonsoft.Json;
using System.Windows;
using LunaForge.EditorData.Project;
using System.IO.Compression;
using LunaForge.Plugins;
using LunaForge.EditorData.Nodes;
using TreeNode = LunaForge.EditorData.Nodes.TreeNode;

namespace LunaForge.GUI;

/*
 * LunaForge - *.lfp (LunaForge Project) for the ini file. - *.lfd (LunaForge Definition) for object definitions.
 * 
 * Design:
 * A project has a .ini file for the configuration and everything else.
 * 
 * A project is NOT a single file but an entire folder.
 * Each object definition is a lua file.
 * 
 * There's a "Project Tree" window will all files.
 * 
 * Each project is a window instance.
 * 
 * Templates are in a zip format and unzipped at creation.
 * Have a single file for the entry point (main menu? (with an option to hijack the launcher.lua?))
 * 
 * 
 * Plugin system:
 * Plugin manager, loaded at startup. Can add nodes or other features.
 * An interface entry plugin scanned with Reflection and instanciated.
 * Plugin window in config to enable/disable them with warning like "can allow malicious code to run"
 * 
 * 
 * Definitions:
 * The typical TreeNode architecture. Most of the software uses these. Every file definition is a single object.
 * One definition can be the entry point of the game.
 * There a node "Import Definition" to allow the editor to allow the user to instanciate this definition. "Create Object"
 * 
 * 
 * Scripts:
 * Pure lua scripts. Can be opened by the editor as code.
 */

public enum InsertMode
{
    Parent,
    Before,
    Child,
    After,
}

public sealed class MainWindow
{
    public static readonly string LunaForgeName = $"LunaForge Editor";
    public Version? VersionNumber = Assembly.GetEntryAssembly()?.GetName().Version;

    public PluginManager Plugins { get; private set; } = new();

    #region Windows

    public ToolboxWindow ToolboxWin;
    public NodeAttributeWindow NodeAttributeWin;
    public DefinitionsWindow DefinitionsWin;
    public MessagesWindow MessagesWin;
    public DebugLogWindow DebugLogWin;
    public NewProjWindow NewProjWin;
    public FileSystemWindow FSWin;

    #endregion
    #region Properties

    public InsertMode InsertMode { get; set; } = InsertMode.Child;

    public Dictionary<string, Texture2D> EditorImages = [];

    public ProjectCollection Workspaces;

    #endregion

    public MainWindow()
    {
        ToolboxWin = new(this);
        NodeAttributeWin = new(this);
        DefinitionsWin = new(this);
        MessagesWin = new(this);
        DebugLogWin = new(this);
        NewProjWin = new(this);
        FSWin = new(this);

        Workspaces = new(this);
    }

    public void Initialize()
    {
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 800, $"{LunaForgeName} v{VersionNumber}");
        Raylib.SetExitKey(KeyboardKey.Null);
        Raylib.MaximizeWindow();
        Raylib.SetTargetFPS(60);

        LoadEditorImages();
        NodeManager.RegisterDefinitionNodes();

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        ShortcutList.RegisterShortcuts(this);

        Plugins.LoadPlugins();

        while (!Raylib.WindowShouldClose())
        {
            try
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                rlImGui.Begin();

                ImGui.DockSpaceOverViewport();
                ShortcutList.CheckKeybinds();

                RenderMenu();
                ImGui.ShowDemoWindow();
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
        ToolboxWin.Render();
        NodeAttributeWin.Render();
        lock (Workspaces) // uh.
        {
            foreach (LunaForgeProject? proj in Workspaces.ToList())
                proj?.Window?.Render();
        }
        DefinitionsWin.Render();
        MessagesWin.Render();
        DebugLogWin.Render();
        NewProjWin.Render();
        FSWin.Render();
    }

    #region RenderMenu

    private void RenderMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N"))
                    NewProj();
                if (ImGui.MenuItem("Open", "Ctrl+O"))
                    OpenProj();
                ImGui.Separator();
                if (ImGui.MenuItem("Save", "Ctrl+S", false, Workspaces.Current?.CurrentProjectFile != null))
                    SaveActiveProjectFile();
                if (ImGui.MenuItem("Save As", string.Empty, false, Workspaces.Current?.CurrentProjectFile != null))
                    SaveActiveProjectFileAs();
                ImGui.Separator();
                ImGui.MenuItem("Close", "Ctrl+Q");
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
                ImGui.MenuItem("Ban");
                ImGui.Separator();
                ImGui.MenuItem("Delete");
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
            if (ImGui.BeginMenu("Presets"))
            {
                if (ImGui.MenuItem("Create template from project"))
                    ProjectToTemplate();
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
                ImGui.MenuItem("Documentation");
                ImGui.Separator();
                ImGui.MenuItem("Check for Updates");
                ImGui.MenuItem("About");
                ImGui.EndMenu();
            }
            ImGui.EndMainMenuBar();
        }
    }

    #endregion
    #region Editor Exec

    public void LoadEditorImages()
    {
        EditorImages = [];
        EditorImages.Add("Unknown", Raylib.LoadTexture(Path.Combine(Directory.GetCurrentDirectory(), "Images/Unknown.png")));
    }

    public Texture2D FindTexture(string name)
    {
        if (EditorImages.ContainsKey(name))
            return EditorImages[name];
        else
            return EditorImages["Unknown"];
    }

    public void CreateNewProject()
    {
        string pathToTemplate = Path.GetFullPath(NewProjWin.SelectedPath);

        Thread dialogThread = new(() =>
        {
            string lastUsedPath = LastUsedPath;
            FolderBrowserDialog dialog = new()
            {
                InitialDirectory = string.IsNullOrEmpty(lastUsedPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : lastUsedPath,
                ShowHiddenFiles = true,
                ShowNewFolderButton = true,
                ShowPinnedPlaces = true
            };
            if (dialog.ShowDialog() != DialogResult.OK) return;
            LastUsedPath = dialog.SelectedPath;
            CloneTemplate(pathToTemplate, dialog.SelectedPath);
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
    }

    public async void CloneTemplate(string pathToTemplate, string pathToFolder)
    {
        try
        {
#if DEBUG
            // No template, debug only
            if (pathToTemplate.Contains("specialCommand|=GenerateEmpty"))
            {
                LunaForgeProject newProjDebug = new(NewProjWin, Path.Combine(pathToFolder, NewProjWin.ProjectName));
                Workspaces.Add(newProjDebug);
                newProjDebug.TryGenerateProject();
                return; // Return only for debug to avoid cloning the template.
            }
#endif
            await Task.Factory.StartNew(() =>
            {
                ZipFile.ExtractToDirectory(pathToTemplate, Path.Combine(pathToFolder, NewProjWin.ProjectName));
            });
            string pathToLFP = Path.Combine(pathToFolder, NewProjWin.ProjectName, "Project.lfp");
            Console.WriteLine(pathToLFP);
            LunaForgeProject newProj = LunaForgeProject.CreateFromFile(pathToLFP);
            newProj.ResetVariables(NewProjWin);
            newProj.Save();
            Workspaces.Add(newProj);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public void OpenProject()
    {
        Thread dialogThread = new(() =>
        {
            string lastUsedPath = Properties.Settings.Default.LastUsedPath;
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "LunaForge Project (*.lfp)|*.lfp",
                InitialDirectory = string.IsNullOrEmpty(lastUsedPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : lastUsedPath
            };
            if (openFileDialog.ShowDialog() != true) return;
            for (int i = 0; i < openFileDialog.FileNames.Length; i++)
            {
                if (!IsProjectOpened(openFileDialog.FileNames[i]))
                    OpenProjectFromPath(openFileDialog.SafeFileNames[i], openFileDialog.FileNames[i]);
                Properties.Settings.Default.LastUsedPath = Path.GetDirectoryName(openFileDialog.FileNames[i]);
            }
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
    }

    public bool IsProjectOpened(string path) => Workspaces.Any(x => x.PathToLFP == path);

    public LunaForgeProject? OpenProjectFromPath(string name, string path)
    {
        try
        {
            LunaForgeProject proj = LunaForgeProject.CreateFromFile(path);
            Workspaces.Add(proj);
            return proj;
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public bool CloseProject(LunaForgeProject proj)
    {
        bool allClosed = true;
        foreach (LunaProjectFile file in proj.ProjectFiles.ToList())
        {
            if (!CloseProjectFile(file))
                allClosed = false;
        }
        if (allClosed)
        {
            Workspaces.Remove(proj);
            Workspaces.Current = null;
            return true;
        }
        return false;
    }

    public bool CloseProjectFile(LunaProjectFile file)
    {
        if (file.IsUnsaved)
        {
            switch (System.Windows.MessageBox.Show($"Do you want to save \"{file.FileName}\"?",
                    LunaForgeName, MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    if (SaveProject(file))
                    {
                        Workspaces.Current!.ProjectFiles.Remove(file);
                        Workspaces.Current!.CurrentProjectFile = null;
                    }
                    return true;
                case MessageBoxResult.No:
                    break;
                default:
                    return false;
            }
        }
        else
        {
            Workspaces.Current!.ProjectFiles.Remove(file);
            Workspaces.Current!.CurrentProjectFile = null;
        }
        return true;
    }

    #endregion
    #region Events

    public void NewProj()
    {
        NewProjWin.ResetAndShow();
    }

    public void OpenProj()
    {
        OpenProject();
    }

    #endregion
    #region Compile



    #endregion
    #region TreeNode Infos



    #endregion
    #region Project Operation

    public bool SaveActiveProjectFile() => SaveProject(Workspaces.Current?.CurrentProjectFile);
    public bool SaveActiveProjectFileAs() => SaveProject(Workspaces.Current?.CurrentProjectFile, true);

    public bool SaveProject(LunaProjectFile projFile, bool saveAs = false)
    {
        return projFile.Save(saveAs);
    }

    public bool ProjectToTemplate()
    {
        LunaForgeProject? currentProj = Workspaces.Current;
        if (currentProj == null)
            return false;

        try
        {
            string pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "Templates", $"{currentProj.ProjectName}.zip");
            ZipFile.CreateFromDirectory(currentProj.PathToProjectRoot, pathToFile);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    #endregion
    #region Node Operation

    public bool Insert(TreeNode node, bool doInvoke = true)
    {
        try
        {
            return (Workspaces.Current?.CurrentProjectFile as LunaDefinition).Insert(node, doInvoke);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    #endregion
    #region Configurations

    public bool DefinitionsWindowOpen
    {
        get => Properties.Settings.Default.DefinitionsWindowOpen;
        set => Properties.Settings.Default.DefinitionsWindowOpen = value;
    }

    public string AuthorName
    {
        get => Properties.Settings.Default.AuthorName;
        set => Properties.Settings.Default.AuthorName = value;
    }

    public string LastUsedPath
    {
        get => Properties.Settings.Default.LastUsedPath;
        set => Properties.Settings.Default.LastUsedPath = value;
    }

    #endregion
}
