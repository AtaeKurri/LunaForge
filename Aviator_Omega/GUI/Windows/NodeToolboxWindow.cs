using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Aviator_Omega.GUI.Helpers;

namespace Aviator_Omega.GUI.Windows;

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
