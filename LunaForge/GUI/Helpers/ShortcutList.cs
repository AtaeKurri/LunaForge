using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LunaForge.GUI.Helpers;

public static class ShortcutList
{
    public static MainWindow MainWin { get; private set; }

    public static List<Shortcut> Shortcuts = [];

    #region Shortcuts

    public static Shortcut NewShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.N,
        (obj) => { MainWin.NewProj(); },
        () => { return MainWin.NewProj_CanExecute(); }
    );

    public static Shortcut OpenShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.O,
        (obj) => { MainWin.OpenProj(); },
        () => { return MainWin.OpenProj_CanExecute(); }
    );

    public static Shortcut SaveShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.S,
        (obj) => { MainWin.SaveActiveProjectFile(); },
        () => { return MainWin.SaveActiveProjectFile_CanExecute(); }
    );

    public static Shortcut UndoShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        [ImGuiKey.Z, ImGuiKey.W],
        (obj) => { MainWin.Undo(); },
        () => { return MainWin.Undo_CanExecute(); }
    );

    public static Shortcut RedoShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.Y,
        (obj) => { MainWin.Redo(); },
        () => { return MainWin.Redo_CanExecute(); }
    );

    public static Shortcut InsertBeforeShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.UpArrow,
        (obj) => { MainWin.InsertMode = InsertMode.Before; },
        () => true
    );

    public static Shortcut InsertChildShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.RightArrow,
        (obj) => { MainWin.InsertMode = InsertMode.Child; },
        () => true
    );

    public static Shortcut InsertAfterShortcut = new(
        () => ImGui.GetIO().KeyAlt,
        ImGuiKey.DownArrow,
        (obj) => { MainWin.InsertMode = InsertMode.After; },
        () => true
    );

    public static Shortcut DeleteShortcut = new(
        () => true,
        ImGuiKey.Delete,
        (opj) => { MainWin.Delete(); },
        () => { return MainWin.Delete_CanExecute(); }
    );

    #endregion

    public static void RegisterShortcuts(MainWindow mainWin)
    {
        MainWin = mainWin;
        Shortcuts.Add(NewShortcut);
        Shortcuts.Add(OpenShortcut);
        Shortcuts.Add(SaveShortcut);
        Shortcuts.Add(UndoShortcut);
        Shortcuts.Add(RedoShortcut);
        Shortcuts.Add(InsertBeforeShortcut);
        Shortcuts.Add(InsertChildShortcut);
        Shortcuts.Add(InsertAfterShortcut);
        Shortcuts.Add(DeleteShortcut);
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