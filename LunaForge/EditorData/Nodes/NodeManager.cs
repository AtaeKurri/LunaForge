using LunaForge.API.Core;
using LunaForge.EditorData.Nodes.NodeData.Stages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

public static class NodeManager
{
    public static Dictionary<Type, string> DefinitionNodes { get; set; } = [];

    public static void RegisterDefinitionNodes()
    {
        DefinitionNodes.Add(typeof(StageGroupDefinition), "Stage Group");
    }
}
