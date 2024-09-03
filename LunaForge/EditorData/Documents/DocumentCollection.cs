using LunaForge.EditorData.Nodes.NodeData;
using LunaForge.GUI;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Documents;

public class DocumentCollection : Dictionary<string, LunaForgeDocument>
{
    public int MaxHash { get; private set; } = 0;
    public string SelectedTab = null;
    
    /// <summary>
    /// ID is <see cref="LunaForgeDocument.RawDocName"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="document"></param>
    public void AddAndAllocHash(LunaForgeDocument document, MainWindow mainWin)
    {
        base.Add(document.RawDocName, document);
        document.Parent = this;
        document.Hash = MaxHash;
        document.MainWin = mainWin;
        MaxHash++;
    }

    public LunaForgeDocument GenerateEmptyDocument(string name, ProjectConfiguration conf, MainWindow mainWin)
    {
        LunaForgeDocument doc = new(name, string.Empty, conf, true);
        doc.TreeNodes.Add(new RootNode(doc)); // TODO: this really works?
        doc.TreeNodes[0].AddChild(new RootNode(doc));
        AddAndAllocHash(doc, mainWin);
        return doc;
    }

    public LunaForgeDocument GetSelectedWorkspace()
    {
        if (SelectedTab != null && this.ContainsKey(SelectedTab))
            return this[SelectedTab];
        return null;
    }
}
