using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Project;

public class ProjectCollection : Dictionary<string, LunaForgeProject>
{
    public string SelectedProject = string.Empty;
}
