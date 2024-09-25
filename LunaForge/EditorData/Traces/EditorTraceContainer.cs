using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Traces;

public static class EditorTraceContainer
{
    /*public delegate void EditorTraceEventHandler(ITraceThrowable source, EventArgs e);

    public static event EditorTraceEventHandler OnUpdateTraces;*/

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

    /// <summary>
    /// Checks if traces contains a <see cref="TraceSeverity"/> value.<br/>
    /// Checks traces from only <paramref name="source"/> if it's not null.
    /// </summary>
    /// <param name="severity">The trace severity to check.</param>
    /// <param name="source">Optional source in which to check severity.</param>
    /// <returns>True if traces contains <paramref name="severity"/>; otherwise, false.</returns>
    public static bool ContainSeverity(TraceSeverity severity, ITraceThrowable? source = null)
    {
        foreach (EditorTrace trace in (source == null) ? Traces : Traces.Where(x => x.Source == source))
            if (trace.Severity == severity)
                return true;
        return false;
    }
}
