using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Helpers;

/// <summary>
/// Implementation of ImRaii courtesy of https://github.com/goatcorp/Dalamud
/// All rights belongs to ImGui and Dalamud for the entire code contained in this file.
/// I only edited out the ImPlotNet part because I don't need/want it.
/// </summary>
public static class ImRaii
{
    public sealed class Color : IDisposable
    {
        internal static readonly List<(ImGuiCol, uint)> Stack = new List<(ImGuiCol, uint)>();

        private int count;

        public Color Push(ImGuiCol idx, uint color, bool condition = true)
        {
            if (condition)
            {
                Stack.Add((idx, ImGui.GetColorU32(idx)));
                ImGui.PushStyleColor(idx, color);
                count++;
            }

            return this;
        }

        public Color Push(ImGuiCol idx, Vector4 color, bool condition = true)
        {
            if (condition)
            {
                Stack.Add((idx, ImGui.GetColorU32(idx)));
                ImGui.PushStyleColor(idx, color);
                count++;
            }

            return this;
        }

        public void Pop(int num = 1)
        {
            num = Math.Min(num, count);
            count -= num;
            ImGui.PopStyleColor(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose()
        {
            Pop(count);
        }
    }

    public interface IEndObject : IDisposable
    {
        static readonly IEndObject Empty = new EndConditionally(Nop, success: false);

        bool Success { get; }

        static bool operator true(IEndObject i)
        {
            return i.Success;
        }

        static bool operator false(IEndObject i)
        {
            return !i.Success;
        }

        static bool operator !(IEndObject i)
        {
            return !i.Success;
        }

        static bool operator &(IEndObject i, bool value)
        {
            return i.Success && value;
        }

        static bool operator |(IEndObject i, bool value)
        {
            return i.Success || value;
        }
    }

    private struct EndUnconditionally : IEndObject, IDisposable
    {
        private Action EndAction { get; }

        public bool Success { get; }

        public bool Disposed { get; private set; }

        public EndUnconditionally(Action endAction, bool success)
        {
            EndAction = endAction;
            Success = success;
            Disposed = false;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                EndAction();
                Disposed = true;
            }
        }
    }

    private struct EndConditionally : IEndObject, IDisposable
    {
        public bool Success { get; }

        public bool Disposed { get; private set; }

        private Action EndAction { get; }

        public EndConditionally(Action endAction, bool success)
        {
            EndAction = endAction;
            Success = success;
            Disposed = false;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                if (Success)
                {
                    EndAction();
                }

                Disposed = true;
            }
        }
    }

    public sealed class Font : IDisposable
    {
        internal static int FontPushCounter;

        internal static ImFontPtr DefaultPushed;

        private int count;

        public Font()
        {
            count = 0;
        }

        public Font Push(ImFontPtr font, bool condition = true)
        {
            if (condition)
            {
                if (FontPushCounter++ == 0)
                {
                    DefaultPushed = ImGui.GetFont();
                }

                ImGui.PushFont(font);
                count++;
            }

            return this;
        }

        public void Pop(int num = 1)
        {
            num = Math.Min(num, count);
            count -= num;
            FontPushCounter -= num;
            while (num-- > 0)
            {
                ImGui.PopFont();
            }
        }

        public void Dispose()
        {
            Pop(count);
        }
    }

    public sealed class Id : IDisposable
    {
        private int count;

        public Id Push(string id, bool condition = true)
        {
            if (condition)
            {
                ImGui.PushID(id);
                count++;
            }

            return this;
        }

        public Id Push(int id, bool condition = true)
        {
            if (condition)
            {
                ImGui.PushID(id);
                count++;
            }

            return this;
        }

        public Id Push(nint id, bool condition = true)
        {
            if (condition)
            {
                ImGui.PushID(id);
                count++;
            }

            return this;
        }

        public void Pop(int num = 1)
        {
            num = Math.Min(num, count);
            count -= num;
            while (num-- > 0)
            {
                ImGui.PopID();
            }
        }

        public void Dispose()
        {
            Pop(count);
        }
    }

    public sealed class Style : IDisposable
    {
        internal static readonly List<(ImGuiStyleVar, Vector2)> Stack = new List<(ImGuiStyleVar, Vector2)>();

        private int count;

        private static void CheckStyleIdx(ImGuiStyleVar idx, Type type)
        {
            if (idx switch
            {
                ImGuiStyleVar.Alpha => type != typeof(float),
                ImGuiStyleVar.WindowPadding => type != typeof(Vector2),
                ImGuiStyleVar.WindowRounding => type != typeof(float),
                ImGuiStyleVar.WindowBorderSize => type != typeof(float),
                ImGuiStyleVar.WindowMinSize => type != typeof(Vector2),
                ImGuiStyleVar.WindowTitleAlign => type != typeof(Vector2),
                ImGuiStyleVar.ChildRounding => type != typeof(float),
                ImGuiStyleVar.ChildBorderSize => type != typeof(float),
                ImGuiStyleVar.PopupRounding => type != typeof(float),
                ImGuiStyleVar.PopupBorderSize => type != typeof(float),
                ImGuiStyleVar.FramePadding => type != typeof(Vector2),
                ImGuiStyleVar.FrameRounding => type != typeof(float),
                ImGuiStyleVar.FrameBorderSize => type != typeof(float),
                ImGuiStyleVar.ItemSpacing => type != typeof(Vector2),
                ImGuiStyleVar.ItemInnerSpacing => type != typeof(Vector2),
                ImGuiStyleVar.IndentSpacing => type != typeof(float),
                ImGuiStyleVar.CellPadding => type != typeof(Vector2),
                ImGuiStyleVar.ScrollbarSize => type != typeof(float),
                ImGuiStyleVar.ScrollbarRounding => type != typeof(float),
                ImGuiStyleVar.GrabMinSize => type != typeof(float),
                ImGuiStyleVar.GrabRounding => type != typeof(float),
                ImGuiStyleVar.TabRounding => type != typeof(float),
                ImGuiStyleVar.ButtonTextAlign => type != typeof(Vector2),
                ImGuiStyleVar.SelectableTextAlign => type != typeof(Vector2),
                ImGuiStyleVar.DisabledAlpha => type != typeof(float),
                _ => throw new ArgumentOutOfRangeException("idx", idx, null),
            })
            {
                throw new ArgumentException($"Unable to push {type} to {idx}.");
            }
        }

        public static Vector2 GetStyle(ImGuiStyleVar idx)
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            return idx switch
            {
                ImGuiStyleVar.Alpha => new Vector2(style.Alpha, float.NaN),
                ImGuiStyleVar.WindowPadding => style.WindowPadding,
                ImGuiStyleVar.WindowRounding => new Vector2(style.WindowRounding, float.NaN),
                ImGuiStyleVar.WindowBorderSize => new Vector2(style.WindowBorderSize, float.NaN),
                ImGuiStyleVar.WindowMinSize => style.WindowMinSize,
                ImGuiStyleVar.WindowTitleAlign => style.WindowTitleAlign,
                ImGuiStyleVar.ChildRounding => new Vector2(style.ChildRounding, float.NaN),
                ImGuiStyleVar.ChildBorderSize => new Vector2(style.ChildBorderSize, float.NaN),
                ImGuiStyleVar.PopupRounding => new Vector2(style.PopupRounding, float.NaN),
                ImGuiStyleVar.PopupBorderSize => new Vector2(style.PopupBorderSize, float.NaN),
                ImGuiStyleVar.FramePadding => style.FramePadding,
                ImGuiStyleVar.FrameRounding => new Vector2(style.FrameRounding, float.NaN),
                ImGuiStyleVar.FrameBorderSize => new Vector2(style.FrameBorderSize, float.NaN),
                ImGuiStyleVar.ItemSpacing => style.ItemSpacing,
                ImGuiStyleVar.ItemInnerSpacing => style.ItemInnerSpacing,
                ImGuiStyleVar.IndentSpacing => new Vector2(style.IndentSpacing, float.NaN),
                ImGuiStyleVar.CellPadding => style.CellPadding,
                ImGuiStyleVar.ScrollbarSize => new Vector2(style.ScrollbarSize, float.NaN),
                ImGuiStyleVar.ScrollbarRounding => new Vector2(style.ScrollbarRounding, float.NaN),
                ImGuiStyleVar.GrabMinSize => new Vector2(style.GrabMinSize, float.NaN),
                ImGuiStyleVar.GrabRounding => new Vector2(style.GrabRounding, float.NaN),
                ImGuiStyleVar.TabRounding => new Vector2(style.TabRounding, float.NaN),
                ImGuiStyleVar.ButtonTextAlign => style.ButtonTextAlign,
                ImGuiStyleVar.SelectableTextAlign => style.SelectableTextAlign,
                ImGuiStyleVar.DisabledAlpha => new Vector2(style.DisabledAlpha, float.NaN),
                _ => throw new ArgumentOutOfRangeException("idx", idx, null),
            };
        }

        public Style Push(ImGuiStyleVar idx, float value, bool condition = true)
        {
            if (!condition)
            {
                return this;
            }

            CheckStyleIdx(idx, typeof(float));
            Stack.Add((idx, GetStyle(idx)));
            ImGui.PushStyleVar(idx, value);
            count++;
            return this;
        }

        public Style Push(ImGuiStyleVar idx, Vector2 value, bool condition = true)
        {
            if (!condition)
            {
                return this;
            }

            CheckStyleIdx(idx, typeof(Vector2));
            Stack.Add((idx, GetStyle(idx)));
            ImGui.PushStyleVar(idx, value);
            count++;
            return this;
        }

        public void Pop(int num = 1)
        {
            num = Math.Min(num, count);
            count -= num;
            ImGui.PopStyleVar(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose()
        {
            Pop(count);
        }
    }

    private static int disabledCount;

    public static Color PushColor(ImGuiCol idx, uint color, bool condition = true)
    {
        return new Color().Push(idx, color, condition);
    }

    public static Color PushColor(ImGuiCol idx, Vector4 color, bool condition = true)
    {
        return new Color().Push(idx, color, condition);
    }

    public static Color DefaultColors()
    {
        Color color = new Color();
        (ImGuiCol, uint)[] array = (from p in Color.Stack
                                    group p by p.Item1 into p
                                    select (p.Key, p.First().Item2)).ToArray();
        (ImGuiCol, uint)[] array2 = array;
        for (int i = 0; i < array2.Length; i++)
        {
            var (idx, color2) = array2[i];
            color.Push(idx, color2);
        }

        return color;
    }

    public static IEndObject Child(string strId)
    {
        return new EndUnconditionally(ImGui.EndChild, ImGui.BeginChild(strId));
    }

    public static IEndObject Child(string strId, Vector2 size)
    {
        return new EndUnconditionally(ImGui.EndChild, ImGui.BeginChild(strId, size));
    }

    public static IEndObject DragDropTarget()
    {
        return new EndConditionally(ImGui.EndDragDropTarget, ImGui.BeginDragDropTarget());
    }

    public static IEndObject DragDropSource()
    {
        return new EndConditionally(ImGui.EndDragDropSource, ImGui.BeginDragDropSource());
    }

    public static IEndObject DragDropSource(ImGuiDragDropFlags flags)
    {
        return new EndConditionally(ImGui.EndDragDropSource, ImGui.BeginDragDropSource(flags));
    }

    public static IEndObject Popup(string id)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopup(id));
    }

    public static IEndObject Popup(string id, ImGuiWindowFlags flags)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopup(id, flags));
    }

    public static IEndObject PopupModal(string id)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupModal(id));
    }

    public static IEndObject PopupModal(string id, ref bool open)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupModal(id, ref open));
    }

    public static IEndObject PopupModal(string id, ref bool open, ImGuiWindowFlags flags)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupModal(id, ref open, flags));
    }

    public static IEndObject ContextPopup(string id)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupContextWindow(id));
    }

    public static IEndObject ContextPopup(string id, ImGuiPopupFlags flags)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupContextWindow(id, flags));
    }

    public static IEndObject ContextPopupItem(string id)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupContextItem(id));
    }

    public static IEndObject ContextPopupItem(string id, ImGuiPopupFlags flags)
    {
        return new EndConditionally(ImGui.EndPopup, ImGui.BeginPopupContextItem(id, flags));
    }

    public static IEndObject Combo(string label, string previewValue)
    {
        return new EndConditionally(ImGui.EndCombo, ImGui.BeginCombo(label, previewValue));
    }

    public static IEndObject Combo(string label, string previewValue, ImGuiComboFlags flags)
    {
        return new EndConditionally(ImGui.EndCombo, ImGui.BeginCombo(label, previewValue, flags));
    }

    public static IEndObject Group()
    {
        ImGui.BeginGroup();
        return new EndUnconditionally(ImGui.EndGroup, success: true);
    }

    public static IEndObject Tooltip()
    {
        ImGui.BeginTooltip();
        return new EndUnconditionally(ImGui.EndTooltip, success: true);
    }

    //
    // Résumé :
    //     Pushes the item width for the next widget and returns an IDisposable that pops
    //     the width when done.
    //
    // Paramètres :
    //   width:
    //     The width to set the next widget to.
    //
    // Retourne :
    //     An System.IDisposable for use in a using statement.
    public static IEndObject ItemWidth(float width)
    {
        ImGui.PushItemWidth(width);
        return new EndUnconditionally(ImGui.PopItemWidth, success: true);
    }

    //
    // Résumé :
    //     Pushes the item wrapping width for the next string written and returns an IDisposable
    //     that pops the wrap width when done.
    //
    // Paramètres :
    //   pos:
    //     The wrap width to set the next text written to.
    //
    // Retourne :
    //     An System.IDisposable for use in a using statement.
    public static IEndObject TextWrapPos(float pos)
    {
        ImGui.PushTextWrapPos(pos);
        return new EndUnconditionally(ImGui.PopTextWrapPos, success: true);
    }

    public static IEndObject ListBox(string label)
    {
        return new EndConditionally(ImGui.EndListBox, ImGui.BeginListBox(label));
    }

    public static IEndObject ListBox(string label, Vector2 size)
    {
        return new EndConditionally(ImGui.EndListBox, ImGui.BeginListBox(label, size));
    }

    public static IEndObject Table(string table, int numColumns)
    {
        return new EndConditionally(ImGui.EndTable, ImGui.BeginTable(table, numColumns));
    }

    public static IEndObject Table(string table, int numColumns, ImGuiTableFlags flags)
    {
        return new EndConditionally(ImGui.EndTable, ImGui.BeginTable(table, numColumns, flags));
    }

    public static IEndObject Table(string table, int numColumns, ImGuiTableFlags flags, Vector2 outerSize)
    {
        return new EndConditionally(ImGui.EndTable, ImGui.BeginTable(table, numColumns, flags, outerSize));
    }

    public static IEndObject Table(string table, int numColumns, ImGuiTableFlags flags, Vector2 outerSize, float innerWidth)
    {
        return new EndConditionally(ImGui.EndTable, ImGui.BeginTable(table, numColumns, flags, outerSize, innerWidth));
    }

    public static IEndObject TabBar(string label)
    {
        return new EndConditionally(ImGui.EndTabBar, ImGui.BeginTabBar(label));
    }

    public static IEndObject TabBar(string label, ImGuiTabBarFlags flags)
    {
        return new EndConditionally(ImGui.EndTabBar, ImGui.BeginTabBar(label, flags));
    }

    public static IEndObject TabItem(string label)
    {
        return new EndConditionally(ImGui.EndTabItem, ImGui.BeginTabItem(label));
    }

    public unsafe static IEndObject TabItem(byte* label, ImGuiTabItemFlags flags)
    {
        return new EndConditionally(ImGuiNative.igEndTabItem, ImGuiNative.igBeginTabItem(label, null, flags) != 0);
    }

    public unsafe static IEndObject TabItem(string label, ImGuiTabItemFlags flags)
    {
        ArgumentNullException.ThrowIfNull(label, "label");
        int byteCount = Encoding.UTF8.GetByteCount(label);
        if (byteCount > 2048)
        {
            throw new ArgumentOutOfRangeException("label", $"Label is too long. (Longer than {2048} bytes)");
        }

        byte* ptr = stackalloc byte[(int)(uint)(byteCount + 1)];
        int bytes;
        fixed (char* chars = label)
        {
            bytes = Encoding.UTF8.GetBytes(chars, label.Length, ptr, byteCount);
        }

        ptr[bytes] = 0;
        byte b = ImGuiNative.igBeginTabItem(ptr, null, flags);
        return new EndConditionally(ImGuiNative.igEndTabItem, b != 0);
    }

    public static IEndObject TabItem(string label, ref bool open)
    {
        return new EndConditionally(ImGui.EndTabItem, ImGui.BeginTabItem(label, ref open));
    }

    public static IEndObject TabItem(string label, ref bool open, ImGuiTabItemFlags flags)
    {
        return new EndConditionally(ImGui.EndTabItem, ImGui.BeginTabItem(label, ref open, flags));
    }

    public static IEndObject TreeNode(string label)
    {
        return new EndConditionally(ImGui.TreePop, ImGui.TreeNodeEx(label));
    }

    public static IEndObject TreeNode(string label, ImGuiTreeNodeFlags flags)
    {
        return new EndConditionally(flags.HasFlag(ImGuiTreeNodeFlags.NoTreePushOnOpen) ? new Action(Nop) : new Action(ImGui.TreePop), ImGui.TreeNodeEx(label, flags));
    }

    public static IEndObject Disabled()
    {
        ImGui.BeginDisabled();
        disabledCount++;
        return DisabledEnd();
    }

    public static IEndObject Disabled(bool disabled)
    {
        if (!disabled)
        {
            return new EndConditionally(Nop, success: false);
        }

        ImGui.BeginDisabled();
        disabledCount++;
        return DisabledEnd();
    }

    public static IEndObject Enabled()
    {
        int oldCount = disabledCount;
        if (oldCount == 0)
        {
            return new EndConditionally(Nop, success: false);
        }

        while (disabledCount > 0)
        {
            ImGui.EndDisabled();
            disabledCount--;
        }

        return new EndUnconditionally(Restore, success: true);
        void Restore()
        {
            disabledCount += oldCount;
            while (--oldCount >= 0)
            {
                ImGui.BeginDisabled();
            }
        }
    }

    private static IEndObject DisabledEnd()
    {
        return new EndUnconditionally(delegate
        {
            disabledCount--;
            ImGui.EndDisabled();
        }, success: true);
    }

    private static void Nop()
    {
    }

    public static Font PushFont(ImFontPtr font, bool condition = true)
    {
        if (!condition)
        {
            return new Font();
        }

        return new Font().Push(font);
    }

    public static Font DefaultFont()
    {
        return new Font().Push(Font.DefaultPushed, Font.FontPushCounter > 0);
    }

    public static Id PushId(string id, bool enabled = true)
    {
        if (!enabled)
        {
            return new Id();
        }

        return new Id().Push(id);
    }

    public static Id PushId(int id, bool enabled = true)
    {
        if (!enabled)
        {
            return new Id();
        }

        return new Id().Push(id);
    }

    public static Id PushId(nint id, bool enabled = true)
    {
        if (!enabled)
        {
            return new Id();
        }

        return new Id().Push(id);
    }

    public static Style PushStyle(ImGuiStyleVar idx, float value, bool condition = true)
    {
        return new Style().Push(idx, value, condition);
    }

    public static Style PushStyle(ImGuiStyleVar idx, Vector2 value, bool condition = true)
    {
        return new Style().Push(idx, value, condition);
    }

    public static Style DefaultStyle()
    {
        Style style = new Style();
        (ImGuiStyleVar, Vector2)[] array = (from p in Style.Stack
                                            group p by p.Item1 into p
                                            select (p.Key, p.First().Item2)).ToArray();
        (ImGuiStyleVar, Vector2)[] array2 = array;
        for (int i = 0; i < array2.Length; i++)
        {
            var (idx, value) = array2[i];
            if (float.IsNaN(value.Y))
            {
                style.Push(idx, value.X);
            }
            else
            {
                style.Push(idx, value);
            }
        }

        return style;
    }
}
