using LunaForge.EditorData.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

/// <summary>
/// <see cref="TreeNode"/> is a Class Node. This attribute will be inherited.<br/>
/// Classe Nodes are Definitions that can be created inside other Definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class ClassNodeAttribute(DefinitionMetaType type) : Attribute
{
    public DefinitionMetaType MetaType { get; set; } = type;
}
