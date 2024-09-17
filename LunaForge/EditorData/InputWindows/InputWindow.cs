using ImGuiNET;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.InputWindows;

public abstract class InputWindow
{
    private Vector2 ModalSize = Vector2.One;

    private Action<string> Callback;

    private bool IsRendering = false;

    public bool ModalResult { get; set; }

    public string Title = string.Empty;

    public string Result;

    protected static List<string> Separate(string s)
    {
        try
        {
            List<string> vs = [];
            int lastlocptr = 0;
            char[] c = s.ToCharArray();
            Stack<char> expr = [];
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == '(' || c[i] == '[' || c[i] == '{')
                {
                    expr.Push(c[i]);
                }
                else if (c[i] == ')' || c[i] == ']' || c[i] == '}')
                {
                    if (expr.Peek() == '(' && c[i] == ')') expr.Pop();
                    else if (expr.Peek() == '[' && c[i] == ']') expr.Pop();
                    else if (expr.Peek() == '{' && c[i] == '}') expr.Pop();
                    else throw new InvalidOperationException();
                }
                else if (c[i] == ',')
                {
                    if (expr.Count == 0)
                    {
                        vs.Add(new string(c, lastlocptr, i - lastlocptr));
                        lastlocptr = i + 1;
                    }
                }
            }
            vs.Add(new string(c, lastlocptr, c.Length - lastlocptr));
            return vs;
        }
        catch (InvalidOperationException)
        {
            return [s];
        }
        catch (NullReferenceException)
        {
            return [];
        }
    }

    protected static bool MatchFilter(string source, string filter)
    {
        return source.Contains(filter);
    }

    public InputWindow(string title, Vector2? modalSize = null)
    {
        Title = title;
        ModalSize = (Vector2)((modalSize != null) ? modalSize : new Vector2(800, 100));
    }

    public void Invoke(Action<string> callback)
    {
        Callback = callback;
        InputWindowSelector.CurrentInputWindow = this;
        IsRendering = true;
    }

    public void AppendTitle(string s)
    {
        Title = $"{s} - {Title}";
    }

    protected void SetModalToCenter()
    {
        Vector2 renderSize = new(Raylib.GetRenderWidth(), Raylib.GetRenderHeight());
        ImGui.SetNextWindowSize(ModalSize);
        ImGui.SetNextWindowPos(renderSize / 2 - (ModalSize / 2));
    }

    private void Close(bool invoke = true)
    {
        if (invoke)
            Callback(Result);
        ImGui.CloseCurrentPopup();
        IsRendering = false;
        InputWindowSelector.CurrentInputWindow = null;
    }

    protected void RenderModalButtons()
    {
        // Set buttons at the bottom.
        float availableHeight = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
        float buttonHeight = ImGui.CalcTextSize("Ok").Y + ImGui.GetStyle().FramePadding.Y * 2;
        float spacing = ImGui.GetStyle().ItemSpacing.Y + 4;
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + availableHeight - buttonHeight - spacing);

        if (ImGui.Button("Ok"))
        {
            Close();
        }
        ImGui.SameLine();
        if (ImGui.Button("Cancel"))
        {
            Close(false);
        }
    }

    protected void CloseOnEnter()
    {
        if (ImGui.IsKeyDown(ImGuiKey.Enter) || ImGui.IsKeyDown(ImGuiKey.KeypadEnter))
        {
            Close();
        }
    }

    public void Render()
    {
        if (IsRendering)
        {
            ImGui.OpenPopup(Title);
            RenderModal();
        }
    }

    public abstract void RenderModal();

    public bool BeginPopupModal()
    {
        return ImGui.BeginPopupModal(Title, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize);
    }
}
