using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Traces;

public static class EditorTraceContainer
{
    public static List<EditorTrace> Traces { get; set; } = [];

    public static void UpdateTraces(ITraceThrowable source)
    {
        var traces = from EditorTrace editorTrace in Traces
                     where editorTrace.Source == source
                     select editorTrace;
        List<EditorTrace> editorTraces = new(traces);
        foreach (EditorTrace et in editorTraces)
            Traces.Remove(et);
        foreach (EditorTrace et in source.Traces)
            Traces.Add(et);
    }

    public static void RemoveChecksFromSource(ITraceThrowable source)
    {
        // Faut faire un truc avec les parents des sources, ça marche pas ça
        var traces = from EditorTrace editorTrace in Traces
                     where editorTrace.Source == source
                     select editorTrace;
        List<EditorTrace> editorTraces = new(traces);
        foreach (EditorTrace et in editorTraces)
            Traces.Remove(et);
    }

    public static bool ContainSeverity(TraceSeverity severity)
    {
        foreach (EditorTrace trace in Traces)
            if (trace.Severity == severity)
                return true;
        return false;
    }
}
