using LunaForge.EditorData.Project;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.Execution;

public delegate void Logger(string s);

public abstract class LSTGExecution
{
    protected abstract LunaForgeProject ProjectExec { get; }

    protected Process LSTGInstance { get; set; }

    protected string Parameters { get; set; }

    protected bool UseShellExecute { get; set; }

    protected bool CreateNoWindow { get; set; }

    protected bool RedirectStandardError { get; set; }

    protected bool RedirectStandardOutput { get; set; }

    protected virtual string WorkingDirectory => Path.GetDirectoryName(ProjectExec.PathToLuaSTGExecutable);

    protected virtual string LuaSTGPath => ProjectExec.PathToLuaSTGExecutable;

    protected abstract string LogFileName { get; }

    protected string ExecutableName => Path.GetFileName(ProjectExec.PathToLuaSTGExecutable);

    public abstract void BeforeRun();

    public virtual void Run(Logger logger, Action end)
    {
        if (LSTGInstance == null || LSTGInstance.HasExited)
        {
            LSTGInstance = new Process
            {
                StartInfo = new ProcessStartInfo(LuaSTGPath, Parameters)
                {
                    UseShellExecute = UseShellExecute,
                    CreateNoWindow = CreateNoWindow,
                    WorkingDirectory = WorkingDirectory,
                    RedirectStandardError = RedirectStandardError,
                    RedirectStandardOutput = RedirectStandardOutput
                },
                EnableRaisingEvents = true
            };
            LSTGInstance.Start();

            logger("LuaSTG is Running.\n\n");

            LSTGInstance.Exited += (s, e) =>
            {
                using FileStream fs = new(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(LuaSTGPath), LogFileName)), FileMode.Open);
                using StreamReader sr = new(fs);
                StringBuilder sb = new();

                try
                {
                    int i = 0;
                    while (!sr.EndOfStream && i < 8192)
                    {
                        sb.Append($"{sr.ReadLine()}\n");
                        i++;
                    }
                    logger(sb.ToString());
                    end();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                sb.Append($"\nExited with code {LSTGInstance.ExitCode}.");
                logger(sb.ToString());
            };
        }
        else
        {
            Console.WriteLine("LuaSTG is already running. Please exit first.");
        }
    }
}
