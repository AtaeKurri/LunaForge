using LunaForge.EditorData.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefaultToolbox.Nodes.Stages;

public class StageDefinition : TreeNode
{
    public override string ToString()
    {
        throw default;
    }

    public override IEnumerable<Tuple<int, TreeNode>> GetLines()
    {
        yield return new Tuple<int, TreeNode>(1, this);
        foreach (var a in GetChildLines())
        {
            yield return a;
        }
        yield return new Tuple<int, TreeNode>(1, this);
    }

    public override IEnumerable<string> ToLua(int spacing)
    {
        foreach (var a in base.ToLua(spacing + 1))
        {
            yield return a;
        }
    }

    public override object Clone()
    {
        return default;
    }
}
