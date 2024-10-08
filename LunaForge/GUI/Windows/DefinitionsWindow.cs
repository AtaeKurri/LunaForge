﻿using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI.Windows;

public class DefinitionsWindow : ImGuiWindow
{
    public DefinitionsWindow()
        : base(Configuration.Default.DefinitionsWindowOpen)
    {

    }

    public override void Render()
    {
        if (Begin("Object Definitions"))
        {
            End();
        }
        Configuration.Default.DefinitionsWindowOpen = ShowWindow;
    }
}
