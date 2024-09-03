using Aviator_Omega.GUI.Helpers;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ImGuiNET;

namespace Aviator_Omega.GUI.Windows;

public class NewDocWindow : ImGuiWindow
{
    public string FileName = "Untitled";
    public string Author;
    public bool AllowPr = true;
    public bool AllowScPr = true;
    public int Modversion = 4096; // For LuaSTG.

    public string SelectedPath = string.Empty;
    private DefS SelectedTemplate = null;

    private class DefS
    {
        public string Text { get; set; }
        public string FullPath { get; set; }
        public string Description { get; set; }
    }

    private List<DefS> Templates;

    public NewDocWindow(MainWindow parent)
        : base(parent, false)
    {
        Author = parent.AuthorName;
    }

    public void Reset()
    {
        ShowWindow = false;
        SelectedTemplate = null;

        string s = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates\\"));
        DirectoryInfo dir = new(s);
        List<FileInfo> fis = new(dir.GetFiles("*.avtr"));

        Templates = new(
            from FileInfo fi in fis
            select new DefS
            {
                Text = Path.GetFileNameWithoutExtension(fi.Name),
                FullPath = fi.FullName,
                Description = GetTemplateDescription(Path.GetFileNameWithoutExtension(fi.Name))
            }
        );
    }

    public void ResetAndShow()
    {
        Reset();
        ShowWindow = true;
    }

    private string GetTemplateDescription(string? sel)
    {
        try
        {
            string fullPathDesc = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "Templates", sel + ".txt"));
            FileStream f = new FileStream(fullPathDesc, FileMode.Open);
            StreamReader sr = new StreamReader(f);
            string desc = sr.ReadToEnd();
            f.Close();
            return desc;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return string.Empty;
        }
    }

    public override void Render()
    {
        if (BeginFlags("New File...", ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoCollapse, new Vector2(800, 450)))
        {
            ImGui.BeginGroup();
            {
                if (ImGui.BeginListBox(""))
                {
                    foreach (DefS def in Templates)
                    {
                        if (ImGui.Selectable(def.Text, SelectedTemplate == def))
                        {
                            SelectedTemplate = def;
                        }
                    }
                    ImGui.EndListBox();
                }

                ImGui.SameLine();

                ImGui.BeginGroup();
                ImGui.Text("Description");
                if (SelectedTemplate?.Description != null)
                    ImGui.TextWrapped(SelectedTemplate?.Description);
                ImGui.EndGroup();
            }
            ImGui.EndGroup();

            ImGui.Separator();
            ImGui.Spacing();

            ImGui.InputText("Name", ref FileName, 128);
            ImGui.InputText("Author", ref Author, 128);

            ImGui.Spacing();
            ImGui.Checkbox("Allow Practice", ref AllowPr);
            ImGui.SameLine();
            ImGui.Checkbox("Allow SC Practice", ref AllowScPr);

            ImGui.Spacing();
            if (ImGui.Button("OK"))
                ClickOk();
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
        SelectedPath = SelectedTemplate?.FullPath;
        ParentWindow.CreateNewDocument();
        Close();
    }

    public void ClickGenerateEmpty()
    {
        SelectedPath = "specialCommand|=GenerateEmpty";
        ParentWindow.CreateNewDocument();
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
