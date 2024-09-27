using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.GUI.Helpers;

namespace LunaForge.EditorData.InputWindows.Windows;

public class CodeInput : InputWindow
{
    public CodeInput(string s)
        : base("Code Input", new Vector2(800, 500))
    {
        Result = s;
    }

    public override void RenderModal()
    {
        SetModalToCenter();
        if (BeginPopupModal())
        {
            ImGuiInputTextFlags flags = ImGuiInputTextFlags.AllowTabInput;
            Vector2 size = ImGui.GetContentRegionAvail() - new Vector2(0, 30);
            ImGui.InputTextMultiline($"##{Title}", ref Result, 10_000_000, size, flags);

            RenderModalButtons();
            //CloseOnEnter();

            ImGui.EndPopup();
        }
    }
}
