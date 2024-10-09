using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins;

internal class PluginLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;
    public string filePath;

    public PluginLoadContext(string pluginPath)
        : base(isCollectible: true)
    {
        _resolver = new(pluginPath);
        filePath = pluginPath;
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
            return LoadFromAssemblyPath(assemblyPath);
        return null;
    }
}
