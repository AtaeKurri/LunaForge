using LunaForge.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.API.Services;

/// <summary>
/// Allows interaction with the built-in node system.
/// </summary>
public interface INodeService
{
    /// <summary>
    /// Register a <see cref="ITreeNode"/> to be used for definitions.
    /// </summary>
    /// <param name="displayName">The Definition Display name on definition selection.</param>
    /// <returns>True if the node has been registered correctly; otherwise, false.</returns>
    public bool RegisterDefinitionNode<T>(string displayName) where T : ITreeNode;
}
