using LunaForge.GUI.Helpers;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImGuiNET;
using Newtonsoft.Json;

namespace LunaForge.GUI.Windows;

public class NewProjWindow : ImGuiWindow
{
    public string ProjectName = "Untitled";
    public string Author = string.Empty;
    public bool AllowPr = true;
    public bool AllowScPr = true;
    public const int Modversion = 4096; // For LuaSTG.

    public string SelectedPath = string.Empty;
    private TemplateDef SelectedTemplate = null;

    private class TemplateDef
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0.0";
        public string ZipPath { get; set; } = string.Empty;
    }

    private List<TemplateDef> Templates;

    public NewProjWindow(MainWindow parent)
        : base(parent, false)
    {
        Author = parent.AuthorName;
    }

    public void Reset()
    {
        ShowWindow = false;
        SelectedTemplate = null;

        string templatePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\"));
        DirectoryInfo dir = new(templatePath);
        List<FileInfo> fis = new(dir.GetFiles("*.json"));

        Templates = new(
            from FileInfo fi in fis where File.Exists(Path.Combine(templatePath, Path.GetFileNameWithoutExtension(fi.Name) + ".zip"))
            select GetTemplateInfo(templatePath, fi)
        );
    }

    public void ResetAndShow()
    {
        Reset();
        ShowWindow = true;
    }

    private TemplateDef? GetTemplateInfo(string templatePath, FileInfo fi)
    {
        try
        {
            
            using (StreamReader sr = fi.OpenText())
            {
                TemplateDef? def = JsonConvert.DeserializeObject<TemplateDef>(sr.ReadToEnd());
                if (def != null)
                    def.ZipPath = Path.Combine(templatePath, Path.GetFileNameWithoutExtension(fi.Name) + ".zip");
                return def;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    public override void Render()
    {
        if (BeginFlags("New Project...", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse, new Vector2(800, 450)))
        {
            ImGui.BeginGroup();
            {
                if (ImGui.BeginListBox(string.Empty))
                {
                    foreach (TemplateDef def in Templates)
                    {
                        if (ImGui.Selectable($"{def.Name}", SelectedTemplate == def))
                        {
                            SelectedTemplate = def;
                        }
                    }
                    ImGui.EndListBox();
                }

                ImGui.SameLine();

                if (SelectedTemplate != null)
                {
                    ImGui.BeginGroup();
                    ImGui.TextWrapped($"{SelectedTemplate.Name} (v{SelectedTemplate.Version})");
                    ImGui.TextWrapped(SelectedTemplate?.Description);
                    ImGui.EndGroup();
                }
            }
            ImGui.EndGroup();

            ImGui.Separator();
            ImGui.Spacing();

            ImGui.InputText("Name", ref ProjectName, 128);
            ImGui.InputText("Author", ref Author, 128);

            ImGui.Spacing();
            ImGui.Checkbox("Allow Practice", ref AllowPr);
            ImGui.SameLine();
            ImGui.Checkbox("Allow SC Practice", ref AllowScPr);

            float availableHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
            float buttonHeight = ImGui.CalcTextSize("OK").Y + ImGui.GetStyle().FramePadding.Y * 2;
            float spacing = ImGui.GetStyle().ItemSpacing.Y + 4;
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + availableHeight - buttonHeight - spacing);

            if (BeginDisabledButton("OK", SelectedTemplate != null))
                ClickOk();
            EndDisabledButton(SelectedTemplate != null);
            ImGui.SameLine();
            if (ImGui.Button("Cancel"))
                ClickCancel();
            ImGui.SameLine();
#if DEBUG
            if (ImGui.Button("Debug:Generate Empty"))
                ClickGenerateEmpty();
#endif
            End();
        }
    }

    public void ClickOk()
    {
        SelectedPath = SelectedTemplate?.ZipPath;
        ParentWindow.CreateNewProject();
        Close();
    }

    public void ClickGenerateEmpty()
    {
        SelectedPath = "specialCommand|=GenerateEmpty";
        ParentWindow.CreateNewProject();
        Close();
    }

    public void ClickCancel()
    {
        Close(true);
    }

    public void Close(bool reset = false)
    {
        if (reset)
            Reset();
        ShowWindow = false;
    }
}
