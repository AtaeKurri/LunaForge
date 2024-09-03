using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using LunaForge.EditorData.Commands;

namespace LunaForge.EditorData.Project;

public class LunaForgeProject
{
    public ProjectSettings Settings { get; set; }

    public string PathToProjectRoot { get; set; }
    public string PathToConfig { get; set; } = string.Empty;

    public bool IsSelected;

    public bool IsUnsaved { get; set; } = false;

    public Stack<Command> CommandStack = [];
    public Stack<Command> UndoCommandStack = [];
    public Command? SavedCommand { get; set; } = null;

    public LunaForgeProject(string rootFolder, string pathToConfig)
    {
        PathToProjectRoot = rootFolder;
        PathToConfig = pathToConfig;
    }

    #region IO

    /// <summary>
    /// Tried and generate the project folder structure and the configuration/project file.ini
    /// </summary>
    /// <returns>False if the project already exists or something went wrong. True if the Project was created.</returns>
    public bool TryGenerateProject()
    {
        if (Directory.Exists(PathToProjectRoot))
            return false;

        try
        {
            // Create folders
            Directory.CreateDirectory(PathToProjectRoot);
            Directory.CreateDirectory(Path.Combine(PathToProjectRoot, "Definitions"));
            Directory.CreateDirectory(Path.Combine(PathToProjectRoot, "Scripts"));

            // Create empty config
            Settings = new();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
        
    }

    #endregion
}
