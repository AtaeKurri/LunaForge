using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class RequireAncestorAttribute(params Type[] types) : Attribute
{
    public Type[] RequiredTypes { get; } = types;
}
