using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData;

public class NodeTypeBinder : ISerializationBinder
{
    private readonly string nameSpace = "LunaForge.EditorData.Nodes.NodeData";

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        assemblyName = null;
        typeName = serializedType.FullName.Replace(nameSpace + ".", string.Empty);
    }

    public Type BindToType(string assemblyName, string typeName)
    {
        var fullTypeName = $"{nameSpace}.{typeName}";
        return Type.GetType(fullTypeName);
    }
}