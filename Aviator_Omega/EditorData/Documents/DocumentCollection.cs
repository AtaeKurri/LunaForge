using Aviator_Omega.EditorData.Nodes.NodeData;
using Aviator_Omega.GUI;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aviator_Omega.EditorData.Documents;

public class DocumentCollection : Dictionary<string, AviatorDocument>
{
    public int MaxHash { get; private set; } = 0;
    public string SelectedTab = null;
    
    /// <summary>
    /// ID is <see cref="AviatorDocument.RawDocName"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="document"></param>
    public void AddAndAllocHash(AviatorDocument document, MainWindow mainWin)
    {
        base.Add(document.RawDocName, document);
        document.Parent = this;
        document.Hash = MaxHash;
        document.MainWin = mainWin;
        MaxHash++;
    }

    public AviatorDocument GenerateEmptyDocument(string name, ProjectConfiguration conf, MainWindow mainWin)
    {
        AviatorDocument doc = new(name, string.Empty, conf, true);
        doc.TreeNodes.Add(new RootNode(doc)); // TODO: this really works?
        doc.TreeNodes[0].AddChild(new RootNode(doc));
        AddAndAllocHash(doc, mainWin);
        return doc;
    }

    public AviatorDocument GetSelectedWorkspace()
    {
        if (SelectedTab != null && this.ContainsKey(SelectedTab))
            return this[SelectedTab];
        return null;
    }
}
