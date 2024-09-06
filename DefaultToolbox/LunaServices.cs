using LunaForge.API.Attributes;
using LunaForge.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultToolbox;

public static class LunaServices
{
    [PluginService]
    public static IToolboxService ToolboxService { get; set; }

    [PluginService]
    public static INodeService NodeService { get; set; }
}
