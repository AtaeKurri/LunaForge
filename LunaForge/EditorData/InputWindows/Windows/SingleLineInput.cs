using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.GUI.Helpers;

namespace LunaForge.EditorData.InputWindows.Windows;

public class SingleLineInput : InputWindow
{
    public SingleLineInput(string s)
        : base("Single Line Input")
    {
        Result = s;
    }

    public override void RenderModal()
    {
        SetModalToCenter();
        if (BeginPopupModal())
        {
            ImGui.InputText($"##{Title}", ref Result, 1024);

            RenderModalButtons();
            CloseOnEnter();

            ImGui.EndPopup();
        }
    }
}
