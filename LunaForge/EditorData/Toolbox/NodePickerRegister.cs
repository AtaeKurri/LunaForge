using LunaForge.EditorData.Project;
using LunaForge.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Toolbox;

public abstract class NodePickerRegister(NodePickerTab tab, MainWindow mainApp)
{
    public readonly MainWindow MainApp = mainApp;
    public NodePickerTab Tab = tab;

    public LunaDefinition Def => MainApp.Workspaces.Current?.CurrentProjectFile as LunaDefinition;

    public abstract NodePickerTab RegisterTab();
}
