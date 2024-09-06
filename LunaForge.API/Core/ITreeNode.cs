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

}
