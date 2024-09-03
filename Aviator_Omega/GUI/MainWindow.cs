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
using Aviator_Omega.GUI.Helpers;
using Aviator_Omega.GUI.Windows;
using System.IO;
using Aviator_Omega.EditorData.Documents;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Collections.ObjectModel;

namespace Aviator_Omega.GUI;

/*
 * Design:
 * A project has a .ini file for the configuration and everything else.
 * 
 * A project is NOT a single file but an entire folder.
 * Each object definition is a lua file.
 * 
 * There's a "Project Tree" window will all files.
 * 
 * Each project is a window instance.
 * For the project window, there's tabs for the ??? "Settings"
 */

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

    public NodeToolboxWindow NodeToolboxWin;
    public NodeAttributeWindow NodeAttributeWin;
    public TreeViewWindow TreeViewWin;
    public DefinitionsWindow DefinitionsWin;
    public MessagesWindow MessagesWin;
    public DebugLogWindow DebugLogWin;
    public NewDocWindow NewDocWin;

    #endregion
    #region Properties

    public InsertMode InsertMode { get; set; }

    public Dictionary<string, Texture2D> EditorImages = [];

    #endregion

    public MainWindow()
    {
        NodeToolboxWin = new(this);
        NodeAttributeWin = new(this);
        TreeViewWin = new(this);
        DefinitionsWin = new(this);
        MessagesWin = new(this);
        DebugLogWin = new(this);
        NewDocWin = new(this);
    }

    public void Initialize()
    {
        Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1280, 800, $"Aviator v{VersionNumber}");
        Raylib.SetExitKey(KeyboardKey.Null);
        Raylib.MaximizeWindow();
        Raylib.SetTargetFPS(60);

        LoadEditorImages();

        rlImGui.Setup(true, true);
        ImGui.GetIO().ConfigWindowsMoveFromTitleBarOnly = true;

        ShortcutList.RegisterShortcuts(this);

        while (!Raylib.WindowShouldClose())
        {
            try
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);

                rlImGui.Begin();

                ImGui.DockSpaceOverViewport();
                ShortcutList.CheckKeybinds();

                RenderMenu();
                //ImGui.ShowDemoWindow();
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
        NodeToolboxWin.Render();
        NodeAttributeWin.Render();
        TreeViewWin.Render();
        DefinitionsWin.Render();
        MessagesWin.Render();
        DebugLogWin.Render();
        NewDocWin.Render();
    }

    #region RenderMenu

    private void RenderMenu()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N"))
                    NewDoc();
                if (ImGui.MenuItem("Open", "Ctrl+O"))
                    OpenDoc();
                ImGui.Separator();
                if (ImGui.MenuItem("Save", "Ctrl+S", false, TreeViewWin.CurrentWorkspace != null))
                    SaveActiveDocument();
                if (ImGui.MenuItem("Save As", string.Empty, false, TreeViewWin.CurrentWorkspace != null))
                    SaveActiveDocumentAs();
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

    public void CreateNewDocument()
    {
        string pathToTemplate = Path.GetFullPath(NewDocWin.SelectedPath);
        CloneTemplate(NewDocWin.FileName, pathToTemplate, NewDocWin.Author);
    }

    public void OpenFile()
    {
        Thread dialogThread = new(() =>
        {
            string lastUsedPath = Properties.Settings.Default.LastUsedPath;
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Multiselect = true,
                Filter = "Aviator Project (*.avtr)|*.avtr",
                InitialDirectory = string.IsNullOrEmpty(lastUsedPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : lastUsedPath
            };
            if (openFileDialog.ShowDialog() != true) return;
            for (int i = 0; i < openFileDialog.FileNames.Length; i++)
            {
                if (!IsFileOpened(openFileDialog.FileNames[i]))
                    OpenDocumentFromPath(openFileDialog.SafeFileNames[i], openFileDialog.FileNames[i]);
                Properties.Settings.Default.LastUsedPath = Path.GetDirectoryName(openFileDialog.FileNames[i]);
            }
        });
        dialogThread.SetApartmentState(ApartmentState.STA); // Set to STA for UI thread
        dialogThread.Start();
    }

    public bool CloseFile(AviatorDocument doc)
    {
        if (doc.IsUnsaved)
        {
            switch (System.Windows.MessageBox.Show($"Do you want to save \"{doc.RawDocName}\"?",
                    "Aviator Editor", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes:
                    if (SaveDocument(doc))
                    {
                        TreeViewWin.Workspaces.Remove(doc.RawDocName);
                    }
                    return true;
                case MessageBoxResult.No:
                    break;
                default:
                    return false;
            }
        }
        TreeViewWin.Workspaces.Remove(doc.RawDocName);
        return true;
    }

    public bool IsFileOpened(string path)
    {
        foreach (AviatorDocument doc in TreeViewWin.Workspaces.Values)
            if (!string.IsNullOrEmpty(doc.DocPath) && Path.GetFullPath(doc.DocPath) == Path.GetFullPath(path))
                return true;
        return false;
    }

    public async void OpenDocumentFromPath(string name, string path)
    {
        try
        {
            AviatorDocument doc = await AviatorDocument.CreateDocumentFromFileAsync(name, path);
            TreeViewWin.Workspaces.AddAndAllocHash(doc, this);
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    public async void CloneTemplate(string name, string path, string author)
    {
        try
        {
            AviatorDocument newDoc = null;
            ProjectConfiguration config = new()
            {
                AuthorName = author
            };
            if (path.Contains("specialCommand|=GenerateEmpty"))
            {
                newDoc = TreeViewWin.Workspaces.GenerateEmptyDocument(name, config, this);
            }
            else
            {
                newDoc = await AviatorDocument.CreateDocumentFromFileAsync(name, path, config);
                TreeViewWin.Workspaces.AddAndAllocHash(newDoc, this);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    #endregion
    #region Events

    public void NewDoc()
    {
        NewDocWin.ResetAndShow();
    }

    public void OpenDoc()
    {
        OpenFile();
    }

    #endregion
    #region Compile



    #endregion
    #region TreeNode Infos



    #endregion
    #region Document Operation

    public bool SaveActiveDocument() => SaveDocument(TreeViewWin.CurrentWorkspace);
    public bool SaveActiveDocumentAs() => SaveDocument(TreeViewWin.CurrentWorkspace, true);

    public bool SaveDocument(AviatorDocument doc, bool saveAs = false)
    {
        return doc.Save(saveAs);
    }

    #endregion
    #region Node Operation



    #endregion
    #region Configurations

    public bool DefinitionsWindowOpen
    {
        get => Properties.Settings.Default.DefinitionsWindowOpen;
        set
        { 
            Properties.Settings.Default.DefinitionsWindowOpen = value;
        }
    }

    public string AuthorName
    {
        get => Properties.Settings.Default.AuthorName;
        set
        {
            Properties.Settings.Default.AuthorName = value;
        }
    }

    #endregion
}
