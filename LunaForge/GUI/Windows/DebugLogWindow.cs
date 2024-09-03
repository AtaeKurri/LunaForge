using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Windows;

public class DebugLogWindow : ImGuiWindow
{
    public DebugLogWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Debug Log"))
        {
            End();
        }
    }
}
