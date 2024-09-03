using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

public static class TreeSerializer
{
    public static readonly JsonSerializerSettings TreeNodeSettings =
        new()
        {
            TypeNameHandling = TypeNameHandling.Auto,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            SerializationBinder = new NodeTypeBinder()
        };

    public static string SerializeTreeNode(TreeNode node)
    {
        return JsonConvert.SerializeObject(node, typeof(TreeNode), TreeNodeSettings);
    }

    public static TreeNode? DeserializeTreeNode(string node)
    {
        return JsonConvert.DeserializeObject(node, typeof(TreeNode), TreeNodeSettings) as TreeNode;
    }
}