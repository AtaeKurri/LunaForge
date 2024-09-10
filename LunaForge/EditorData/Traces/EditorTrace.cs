using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Traces;

/// <summary>
/// Indicate the severity of the message trace. Can be filtered in the message table(?).
/// </summary>
public enum TraceSeverity
{
    Info, // Infos from nodes, allows to build or do anything else. 
    Warning, // Warning from nodes, will allow to build but display a message asking if the user wants to continue.
    Error, // Fatal errors, argument mismatch or node specific errors (missing needed values, ...). Will NOT allow to build.
}

public abstract class EditorTrace : ICloneable
{
    public ITraceThrowable? Source { get; set; }
    public string? SourceName { get; set; }
    public string Trace => ToString();
    public TraceSeverity Severity { get; set; }
    public string Icon => Enum.GetName(typeof(TraceSeverity), Severity);

    public EditorTrace(TraceSeverity severity, ITraceThrowable source)
    {
        Severity = severity;
        Source = source;
        SourceName = source?.ToString();
    }

    public EditorTrace(TraceSeverity severity, string sourceName)
        : this(severity, (ITraceThrowable?)null)
    {
        SourceName = sourceName;
    }

    public EditorTrace(TraceSeverity severity, ITraceThrowable source, string sourceName)
        : this(severity, source)
    {
        SourceName = sourceName;
    }

    public abstract new string ToString();

    public abstract object Clone();
    public abstract void Invoke();
}