using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI;

public enum ToastType
{
    Info,
    Success,
    Warning,
    Error
}

public struct Toast
{
    public string Message;
    public ToastType Type;
    public DateTime TimeAdded;
    public float Duration;
    public Action ClickCallback;
}

public static class NotificationManager
{
    private static readonly List<Toast> toasts = [];

    /// <summary>
    /// Max number of displayed toasts at the same time.
    /// </summary>
    public static int MaxToasts { get; set; } = 5;
    /// <summary>
    /// Maximum display time for a toast (in seconds).
    /// </summary>
    public static float MaximumDuration { get; set; } = 5.0f;

    public static int ToastSize { get; set; } = 300;

    /// <summary>
    /// Adds a new notification toast to be displayed.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <param name="clickCallback">If null, clicking the toast will close it.</param>
    public static void AddToast(
        string message,
        ToastType type = ToastType.Info,
        float duration = 5f,
        Action clickCallback = null)
    {
        if (toasts.Count >= MaxToasts)
            toasts.RemoveAt(0); // Remove oldest if limit is reached.

        toasts.Add(new Toast
        {
            Message = message,
            Type = type,
            TimeAdded = DateTime.Now,
            Duration = duration,
            ClickCallback = clickCallback
        });
    }

    public static void AddToast(Toast toast)
    {
        if (toasts.Count >= MaxToasts)
            toasts.RemoveAt(0);

        toasts.Add(toast);
    }

    public static void Render()
    {
        if (toasts.Count == 0)
            return; // Nothing to do.

        Vector2 pos = new(ImGui.GetIO().DisplaySize.X - ToastSize, 40);

        for (int i = 0; i < toasts.Count; i++)
        {
            var toast = toasts[i];

            if ((DateTime.Now - toast.TimeAdded).TotalSeconds > MathF.Min(MaximumDuration, toast.Duration))
            {
                toasts.RemoveAt(i);
                i--;
                continue;
            }

            ImGui.SetNextWindowPos(pos);
            ImGui.SetNextWindowSize(new Vector2(ToastSize - 10, 50));

            Vector4 bgColor;
            switch (toast.Type)
            {
                case ToastType.Success:
                    bgColor = new Vector4(0.0f, 0.8f, 0.0f, 1.0f); // Green for success
                    break;
                case ToastType.Warning:
                    bgColor = new Vector4(0.8f, 0.8f, 0.0f, 1.0f); // Yellow for warning
                    break;
                case ToastType.Error:
                    bgColor = new Vector4(0.8f, 0.0f, 0.0f, 1.0f); // Red for error
                    break;
                default:
                    bgColor = new Vector4(0.2f, 0.2f, 0.2f, 1.0f); // Default gray for info
                    break;
            }

            ImGui.PushStyleColor(ImGuiCol.WindowBg, bgColor);
            if (ImGui.Begin($"##ToastNotification_{i}",
                ImGuiWindowFlags.NoTitleBar
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.AlwaysAutoResize
                | ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoScrollbar))
            {
                ImGui.TextWrapped(toast.Message);

                if (ImGui.IsWindowHovered())
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    if (toast.ClickCallback != null)
                    {
                        toast.ClickCallback();
                    }
                    else
                    {
                        toasts.RemoveAt(i);
                        i--;
                    }
                }

                ImGui.End();
            }
            
            ImGui.PopStyleColor();

            pos.Y += 60;
        }
    }
}
