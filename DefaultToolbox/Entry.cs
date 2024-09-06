using LunaForge.API.Core;
using LunaForge.API.Services;

namespace DefaultToolbox;

public class Entry : ILunaPlugin
{
    public string Name => "Default Toolbox";
    public string[] Authors => [ "Tania Anehina" ];

    public void Initialize()
    {
        Console.WriteLine(LunaServices.ToolboxService.GetToolboxTab("test"));
    }

    public void Update()
    {
        return;
    }

    public void Dispose()
    {
        return;
    }
}