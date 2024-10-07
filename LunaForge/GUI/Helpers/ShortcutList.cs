using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Helpers;

public static class ShortcutList
{
    public static List<Shortcut> Shortcuts = [];

    #region Shortcuts

    public static Shortcut NewShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.N,
        (obj) => { MainWindow.NewProj(); },
        () => { return MainWindow.NewProj_CanExecute(); }
    );

    public static Shortcut OpenShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.O,
        (obj) => { MainWindow.OpenProj(); },
        () => { return MainWindow.OpenProj_CanExecute(); }
    );

    public static Shortcut SaveShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.S,
        (obj) => { MainWindow.SaveActiveProjectFile(); },
        () => { return MainWindow.SaveActiveProjectFile_CanExecute(); }
    );

    public static Shortcut UndoShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        [ImGuiKey.Z, ImGuiKey.W],
        (obj) => { MainWindow.Undo(); },
        () => { return MainWindow.Undo_CanExecute(); }
    );

    public static Shortcut RedoShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.Y,
        (obj) => { MainWindow.Redo(); },
        () => { return MainWindow.Redo_CanExecute(); }
    );

    public static Shortcut InsertBeforeShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.UpArrow,
        (obj) => { MainWindow.InsertMode = InsertMode.Before; },
        () => true
    );

    public static Shortcut InsertChildShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.RightArrow,
        (obj) => { MainWindow.InsertMode = InsertMode.Child; },
        () => true
    );

    public static Shortcut InsertAfterShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.DownArrow,
        (obj) => { MainWindow.InsertMode = InsertMode.After; },
        () => true
    );

    public static Shortcut DeleteShortcut = new(
        () => true,
        ImGuiKey.Delete,
        (obj) => { MainWindow.Delete(); },
        () => { return MainWindow.Delete_CanExecute(); }
    );

    public static Shortcut CutShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.X,
        (obj) => { MainWindow.CutNode(); },
        () => { return MainWindow.CutNode_CanExecute(); }
    );

    public static Shortcut CopyShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.C,
        (obj) => { MainWindow.CopyNode(); },
        () => { return MainWindow.CopyNode_CanExecute(); }
    );

    public static Shortcut PasteShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.V,
        (obj) => { MainWindow.PasteNode(); },
        () => { return MainWindow.PasteNode_CanExecute(); }
    );

    public static Shortcut RunProjectShortcut = new(
        () => true,
        ImGuiKey.F5,
        (obj) => { MainWindow.RunProject(); },
        () => { return MainWindow.RunProject_CanExecute(); }
     );

    #endregion

    public static void RegisterShortcuts()
    {
        Shortcuts.Add(NewShortcut);
        Shortcuts.Add(OpenShortcut);
        Shortcuts.Add(SaveShortcut);
        Shortcuts.Add(UndoShortcut);
        Shortcuts.Add(RedoShortcut);
        Shortcuts.Add(InsertBeforeShortcut);
        Shortcuts.Add(InsertChildShortcut);
        Shortcuts.Add(InsertAfterShortcut);
        Shortcuts.Add(DeleteShortcut);
        Shortcuts.Add(CutShortcut);
        Shortcuts.Add(CopyShortcut);
        Shortcuts.Add(PasteShortcut);
        Shortcuts.Add(RunProjectShortcut);
    }

    public static void CheckKeybinds()
    {
        foreach (Shortcut? shortcut in Shortcuts)
        {
            shortcut?.Check();
        }
    }
}

public delegate void ParamsAction(params object[] oArgs);
public delegate bool CanBeExecutedAction();

public class Shortcut
{
    public readonly Func<bool> ModifierCheck;
    public ImGuiKey[] Keys;
    public ParamsAction Callback;
    public CanBeExecutedAction CanExecuteCallback;

    private bool WasPressedLastFrame = false;

    public Shortcut(Func<bool> mod, ImGuiKey key, ParamsAction oCallback, CanBeExecutedAction canExecute)
    {
        ModifierCheck = mod;
        Keys = [key];
        Callback = oCallback;
        CanExecuteCallback = canExecute;
    }

    public Shortcut(Func<bool> mod, ImGuiKey[] keys, ParamsAction oCallback, CanBeExecutedAction canExecute)
    {
        ModifierCheck = mod;
        Keys = keys;
        Callback = oCallback;
        CanExecuteCallback = canExecute;
    }

    public bool CanExecute() => CanExecuteCallback();

    public void Check()
    {
        bool isPressed = false;
        foreach (ImGuiKey key in Keys)
            isPressed = ModifierCheck() && ImGui.IsKeyDown(key);

        if (isPressed && !WasPressedLastFrame && CanExecute())
        {
            Callback();
            WasPressedLastFrame = true;
        }

        WasPressedLastFrame = isPressed;
    }
}