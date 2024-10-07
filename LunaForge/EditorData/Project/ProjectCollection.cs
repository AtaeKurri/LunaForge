using LunaForge.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Project;

public class ProjectCollection : List<LunaForgeProject>
{
    public int MaxHash { get; private set; } = 0;
    /// <summary>
    /// The currently selected project. (Selected ProjectViewerWindow)
    /// </summary>
    public LunaForgeProject? Current { get; set; } = null;

    /// <summary>
    /// Creates and modifies a <see cref="LunaForgeProject"/>'s hash and create window context.
    /// </summary>
    /// <param name="proj">The project to add</param>
    public new void Add(LunaForgeProject proj)
    {
        proj.Parent = this;
        proj.Hash = MaxHash;
        proj.Window = new()
        {
            ParentProject = proj
        };
        base.Add(proj);
        MaxHash++;
    }
}
