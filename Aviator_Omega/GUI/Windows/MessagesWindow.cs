using Aviator_Omega.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.GUI.Windows;

public class MessagesWindow : ImGuiWindow
{
    public MessagesWindow(MainWindow parent)
        : base(parent, true)
    {

    }

    public override void Render()
    {
        if (BeginNoClose("Messages"))
        {
            End();
        }
    }
}
