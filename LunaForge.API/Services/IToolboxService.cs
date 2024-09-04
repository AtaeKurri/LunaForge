using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.API.Services;

/// <summary>
/// The service used to interact with the Node Toolbox. Define tabs, nodes, etc.
/// </summary>
public interface IToolboxService
{
    /// <summary>
    /// Gets a Toolbox Tab by its name.
    /// </summary>
    /// <param name="id">The name displayed on the Toolbox tab.</param>
    /// <returns>A ToolboxTab object.</returns>
    public string GetToolboxTab(string id);
}
