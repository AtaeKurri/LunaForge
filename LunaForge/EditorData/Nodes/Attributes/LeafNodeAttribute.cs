using LunaForge.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

/// <summary>
/// Identify a <see cref="ITreeNode"/> cannot have children. This attribute will be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class LeafNodeAttribute : Attribute
{

}
