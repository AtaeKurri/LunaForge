using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Traces;

public interface ITraceThrowable
{
    List<EditorTrace> Traces { get; }
    void CheckTrace();
    List<EditorTrace> GetTraces();
}
