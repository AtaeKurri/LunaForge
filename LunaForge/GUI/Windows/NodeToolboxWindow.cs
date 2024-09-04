using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using LunaForge.GUI.Helpers;

namespace LunaForge.GUI.Windows;

public class NodeToolboxWindow : ImGuiWindow
{
    public NodeToolboxWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Node Toolbox"))
        {
            End();
        }
    }
}
