﻿using Aviator_Omega.GUI.ImGuiHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.GUI.Windows;

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
