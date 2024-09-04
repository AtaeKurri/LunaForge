using LunaForge.API.Core;
using LunaForge.API.Services;

namespace DefaultToolbox;

public class Entry : ILunaPlugin
{
    public string Name => "Default Toolbox";
    public string[] Authors => [ "Tania Anehina" ];

    [PluginService]
    public static IToolboxService ToolboxService { get; set; }

    public void Initialize()
    {
        Console.WriteLine(ToolboxService.GetToolboxTab("test"));
    }

    public void Dispose()
    {
        return;
    }
}
