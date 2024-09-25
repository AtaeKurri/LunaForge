using LunaForge.EditorData.Project;
using LunaForge.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Traces.EditorTraces;

public class ProjectPathNotNullTrace : EditorTrace
{
    public string PathName { get; private set; }

    public ProjectPathNotNullTrace(ITraceThrowable source, string pathName)
        : base(TraceSeverity.Error, source, (source as LunaForgeProject).ProjectName)
    {
        PathName = pathName;
    }

    public override string ToString()
    {
        return $"Path \"{PathName}\" must exist.";
    }

    public override object Clone()
    {
        return new ProjectPathNotNullTrace(Source, PathName);
    }

    public override void Invoke()
    {
        (Source as LunaForgeProject).Window.OpenSettings();
    }
}

