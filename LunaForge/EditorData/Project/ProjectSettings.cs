using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace LunaForge.EditorData.Project;

public class ProjectSettings : IniData
{
    #region Properties

    public string AuthorName
    {
        get => this["Common"]["AuthorName"];
        set => this["Common"]["AuthorName"] = value;
    }
    public string ProjectName
    {
        get => this["Common"]["ProjectName"];
        set => this["Common"]["ProjectName"] = value;
    }

    #endregion

    /// <summary>
    /// Don't use this one. Please.
    /// </summary>
    public ProjectSettings()
        : this("John Dough", "Untitled") { }

    /// <summary>
    /// Newly created Projects only.
    /// </summary>
    /// <param name="author">Author of the project</param>
    /// <param name="name">The displayed project name</param>
    public ProjectSettings(string author, string name)
        : base()
    {
        AuthorName = author;
        ProjectName = name;
    }

    /// <summary>
    /// Already existing Project, just populating with already existing paths.
    /// </summary>
    /// <param name="pathToConfig">Path to the configuration .ini file</param>
    public ProjectSettings(string pathToConfig)
        : base()
    {
        FileIniDataParser parser = new();
        IniData config = parser.ReadFile(pathToConfig);
        this.Merge(config);
    }
}
