using DefaultToolbox.Nodes.Stages;
using LunaForge.API.Attributes;
using LunaForge.API.Core;
using LunaForge.API.Services;

namespace DefaultToolbox;

public class Entry : ILunaPlugin
{
    [PluginService]
    public static IToolboxService ToolboxService { get; set; }

    [PluginService]
    public static INodeService NodeService { get; set; }

    public string Name => "Default Toolbox";
    public string[] Authors => [ "Tania Anehina" ];

    public void Initialize()
    {
        //NodeService.RegisterDefinitionNode<StageDefinition>("Stage");
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