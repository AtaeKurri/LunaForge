using LunaForge.API.Attributes;
using LunaForge.API.Core;
using LunaForge.API.Services;
using LunaForge.EditorData.Nodes;
using LunaForge.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins.Services;

public class NodeService : INodeService
{
    public bool RegisterDefinitionNode<T>(string displayName) where T : ITreeNode
    {
        Type pluginNodeType = typeof(T);
        if (typeof(ITreeNode).IsAssignableFrom(pluginNodeType))
        {
            try
            {
                if (pluginNodeType.GetCustomAttribute(typeof(DefinitionNodeAttribute), true) == null)
                    throw new ArgumentException($"\"{pluginNodeType.Name}\" is not a Definition Node.");

                NodeManager.DefinitionNodes.Add(pluginNodeType, displayName);
                Console.WriteLine($"Node \"{pluginNodeType.FullName}\" registered.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        return false;
    }
}
