using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.GUI.Helpers;

namespace LunaForge.EditorData.InputWindows.Windows;

public class Selector : InputWindow
{
    private int SelectedResult = 0;
    private string[] Items;

    public Selector(string s, string[] items, string title)
        : base(title)
    {
        Result = s;
        Items = items;
    }

    public override void RenderModal()
    {
        SetModalToCenter();
        if (BeginPopupModal())
        {
            ImGuiEx.ComboBox($"##{Title}", ref SelectedResult, ref Result, Items);

            RenderModalButtons();
            CloseOnEnter();

            ImGui.EndPopup();
        }
    }
}
