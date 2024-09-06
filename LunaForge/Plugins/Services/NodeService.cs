using LunaForge.API.Core;
using LunaForge.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins.Services;

public class NodeService : INodeService
{
    public bool RegisterNode<T>() where T : ITreeNode
    {
        Type pluginNodeType = typeof(T);
        if (typeof(ITreeNode).IsAssignableFrom(pluginNodeType))
        {
            Console.WriteLine($"Node {pluginNodeType.Name} registered.");
        }
        return true;
    }
}
