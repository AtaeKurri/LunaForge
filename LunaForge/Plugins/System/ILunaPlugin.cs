using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins.System;

/// <summary>
/// Base interface for LunaForge plugins. Every plugin needs to implement this class.<br/>
/// There can be only one class implementing this interface by assembly.<br/>
/// Plugins are initialized after the window initialization and right before anything is drawn.
/// </summary>
public interface ILunaPlugin
{
    /// <summary>
    /// The entry point of the plugin. This method will be used everytime your plugin is loaded/enabled.
    /// </summary>
    public void Initialize();

    /// <summary>
    /// This method is called every frame.
    /// </summary>
    public void Update();

    /// <summary>
    /// Perform the cleanup part of the plugin's resources and/or loaded parts like Toolboxes.<br/>
    /// This method will be called once at the very end of the plugin's lifespan.
    /// </summary>
    public void Unload();
}
