using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Helpers;

public static class ImGuiEx
{
    public static void ComboBox(string label, ref int currentItem, ref string currentInput, string[] items)
    {
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X - 20);

        ImGui.InputText($"{label}", ref currentInput, 1024);
        ImGui.SameLine(0f, 0f);
        if (ImGui.ArrowButton($"{label}_ArrowCombo", ImGuiDir.Down))
        {
            ImGui.OpenPopup($"{label}_combo");
        }
        if (ImGui.BeginPopup($"{label}_combo"))
        {
            for (int i = 0; i < items.Length; i++)
            {
                bool isSelected = i == currentItem;
                if (ImGui.Selectable(items[i], isSelected))
                {
                    currentItem = i;
                    currentInput = items[i];
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndPopup();
        }
    }

    public static void ClickToCopyText(string text, string? textToCopy = null)
    {
        textToCopy ??= text;

        ImGui.Text(text);
        if (ImGui.IsItemHovered())
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
            if (textToCopy != text)
                ImGui.SetTooltip(textToCopy);
        }
        if (ImGui.IsItemClicked())
            ImGui.SetClipboardText(textToCopy);
    }

    public static void CenteredText(string text)
    {
        CenterCursorForText(text);
        ImGui.TextUnformatted(text);
    }

    public static void CenterCursorForText(string text) => CenterCursorFor(ImGui.CalcTextSize(text).X);

    public static void CenterCursorFor(float itemWidth) => ImGui.SetCursorPosX((int)((ImGui.GetWindowWidth() - itemWidth) / 2));
}
