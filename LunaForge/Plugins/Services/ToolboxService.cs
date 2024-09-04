using LunaForge.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Plugins.Services;

public class ToolboxService : IToolboxService
{
    public string GetToolboxTab(string id)
    {
        return "Returned toolbox";
    }
}
