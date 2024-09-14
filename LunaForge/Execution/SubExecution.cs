using LunaForge.EditorData.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Execution;

public class SubExecution(LunaForgeProject proj) : LSTGExecution
{
    protected override LunaForgeProject ProjectExec => proj;

    public override void BeforeRun()
    {
        Parameters = "\""
            + "start_game=true is_debug=true setting.nosplash=true setting.windowed="
            + ProjectExec.Windowed.ToString().ToLower() + " setting.resx=" + ProjectExec.DebugRes.X
            + " setting.resy=" + ProjectExec.DebugRes.Y + " cheat=" + ProjectExec.Cheat.ToString().ToLower()
            + " updatelib=" + false.ToString().ToLower() + " setting.mod=\'"
            + ProjectExec.ProjectName + "\'\" "
            + (ProjectExec.LogWindowSub ? "--log-window" : "");
        UseShellExecute = false;
        CreateNoWindow = true;
        RedirectStandardError = false;
        RedirectStandardOutput = false;
    }

    protected override string LogFileName => "engine.log";
}
