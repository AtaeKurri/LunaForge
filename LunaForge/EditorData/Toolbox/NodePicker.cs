using LunaForge.EditorData.Nodes.Tabs;
using LunaForge.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Toolbox;

public class NodePicker : IEnumerable<NodePickerTab>
{
    public delegate void AddNode();

    public readonly MainWindow MainApp;

    public List<NodePickerTab> NodePickerTabs = [];
    public Dictionary<string, AddNode> NodeFuncs = [];

    public NodePicker(MainWindow mainApp)
    {
        MainApp = mainApp;
        Initialize();
        InitializeData();
    }

    public void Initialize()
    {
        AddRegister(new TabGeneral(new("General"), MainApp));
        AddRegister(new TabStages(new("Stages"), MainApp));
        AddRegister(new TabProject(new("Project"), MainApp));
    }

    public void InitializeData()
    {
        NodeFuncs = [];
        foreach (NodePickerTab tab in NodePickerTabs)
        {
            foreach (NodePickerItem item in tab)
            {
                if (!item.IsSeparator)
                    NodeFuncs.Add(item.Tag, item.AddNodeMethod);
            }
        }
    }

    /// <summary>
    /// Adds a <see cref="NodePickerTab"/> tab to the collection.
    /// </summary>
    /// <param name="tab">The tab to add.</param>
    public void AddRegister(NodePickerRegister tab)
    {
        NodePickerTabs.Add(tab.RegisterTab());
    }

    public IEnumerator<NodePickerTab> GetEnumerator()
    {
        return NodePickerTabs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
