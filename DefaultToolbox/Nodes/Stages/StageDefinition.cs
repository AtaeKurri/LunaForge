using LunaForge.API.Attributes;
using LunaForge.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultToolbox.Nodes.Stages;

public class StageDefinition : ITreeNode
{
    public IEnumerable<Tuple<int, ITreeNode>> GetLines()
    {
        return default;
    }

    public IEnumerable<string> ToLua(int spacing)
    {
        return default;
    }

    public object Clone()
    {
        return default;
    }
}
