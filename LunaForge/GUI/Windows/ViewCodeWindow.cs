using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace LunaForge.GUI.Windows;

public class ViewCodeWindow : ImGuiWindow
{
    public string CodeContent = string.Empty;

    public ViewCodeWindow(MainWindow parent)
        : base(parent, false)
    { }

    public void ResetAndShow(string code)
    {
        CodeContent = code;
        ShowWindow = true;
    }

    public override void Render()
    {
        if (Begin("View Code"))
        {
            ImGui.InputTextMultiline("##CodeContentText", ref CodeContent, 10_000_000, ImGui.GetContentRegionAvail(), ImGuiInputTextFlags.ReadOnly);
            End();
        }
    }
}
