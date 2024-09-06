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
    /// Register a <see cref="ITreeNode"/> into the editor memory.
    /// </summary>
    /// <returns>True if the node has been registered correctly; otherwise, false.</returns>
    public bool RegisterNode<T>() where T : ITreeNode;
}
