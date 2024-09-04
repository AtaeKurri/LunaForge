using LunaForge.API.Core;
using LunaForge.API.Services;
using LunaForge.Plugins.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins;

public sealed class PluginManager
{
    public List<ILunaPlugin> Plugins { get; private set; }

    private readonly IServiceProvider serviceProvider;

    public PluginManager()
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton<IToolboxService, ToolboxService>();

        serviceProvider = serviceCollection.BuildServiceProvider();

        Plugins = [];
    }

    public void LoadPlugins()
    {
        try
        {
            string pluginDir = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            if (!Directory.Exists(pluginDir))
                Directory.CreateDirectory(pluginDir);

            string[] plugins = Directory.GetFiles(pluginDir, "*.dll");

            foreach (string file in plugins)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(file);

                    Type? pluginType = assembly.GetTypes()
                        .FirstOrDefault(t => typeof(ILunaPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    if (pluginType != null)
                    {
                        ILunaPlugin newPlugin = (ILunaPlugin)Activator.CreateInstance(pluginType);
                        InitializePlugin(newPlugin);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load plugin {file}. Reason:\n{ex}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Failed to locate or create the plugin directory. Reason:\n{ex}");
        }
    }

    private void InitializePlugin(ILunaPlugin plugin)
    {
        Type pluginType = plugin.GetType();
        foreach (var property in pluginType.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            if (property.GetCustomAttribute<PluginServiceAttribute>() != null)
            {
                Type serviceType = property.PropertyType;
                object? service = serviceProvider.GetService(serviceType);
                if (service != null)
                    property.SetValue(null, service);
            }
        }

        plugin.Initialize();
        Plugins.Add(plugin);
    }
}
