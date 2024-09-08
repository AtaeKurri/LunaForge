using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

/// <summary>
/// <see cref="TreeNode"/> is a Definition (root) Node. This attribute will be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class DefinitionNodeAttribute : Attribute
{

}
