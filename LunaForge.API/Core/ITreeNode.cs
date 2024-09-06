using LunaForge.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.API.Core;

/// <summary>
/// Used to implement a TreeNode object. Can then be registered using the <see cref="INodeService"/> service.
/// </summary>
public interface ITreeNode : ICloneable
{
    /// <summary>
    /// Create and returns lua code from this node and children nodes.
    /// </summary>
    /// <param name="spacing">A spacing offset from the last code.</param>
    /// <returns><see cref="IEnumerable{T}"/> of lua code.</returns>
    public IEnumerable<string> ToLua(int spacing);

    /// <summary>
    /// Get the number of lines for this node and the child nodes.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<Tuple<int, ITreeNode>> GetLines();
}
