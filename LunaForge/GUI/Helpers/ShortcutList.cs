using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Helpers;

public static class ShortcutList
{
    public static MainWindow MainWin;

    public static List<Shortcut> Shortcuts = [];

    #region Shortcuts

    public static Shortcut NewShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.N,
        (obj) => { MainWin.NewProj(); }
    );

    public static Shortcut OpenShortcut = new(
        () => ImGui.GetIO().KeyCtrl,
        ImGuiKey.O,
        (obj) => { MainWin.OpenProj(); }
    );

    #endregion

    public static void RegisterShortcuts(MainWindow mainWin)
    {
        MainWin = mainWin;
        Shortcuts.Add(NewShortcut);
        Shortcuts.Add(OpenShortcut);
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

public class Shortcut(Func<bool> mod, ImGuiKey key, ParamsAction oCallback)
{
    public readonly Func<bool> ModifierCheck = mod;
    public ImGuiKey Key = key;
    public ParamsAction Callback = oCallback;

    private bool WasPressedLastFrame = false;

    public void Check()
    {
        bool isPressed = ModifierCheck() && ImGui.IsKeyDown(Key);

        if (isPressed && !WasPressedLastFrame)
        {
            Callback();
            WasPressedLastFrame = true;
        }

        WasPressedLastFrame = isPressed;
    }
}