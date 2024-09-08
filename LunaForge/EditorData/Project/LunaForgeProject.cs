using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunaForge.EditorData.Commands;
using LunaForge.GUI;
using LunaForge.GUI.Windows;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.ComponentModel;

namespace LunaForge.EditorData.Project;

public class LunaForgeProject(NewProjWindow? newProjWin, string rootFolder)
{
    #region Configuration

    [DefaultValue("")]
    public string AuthorName { get; set; } = newProjWin?.Author;
    [DefaultValue("")]
    public string ProjectName { get; set; } = newProjWin?.ProjectName;
    [DefaultValue(false)]
    public bool AllowPr { get; set; } = newProjWin?.AllowPr ?? false;
    [DefaultValue(false)]
    public bool AllowScPr { get; set; } = newProjWin?.AllowScPr ?? false;

    #endregion

    [YamlIgnore]
    public string PathToProjectRoot { get; set; } = rootFolder;
    [YamlIgnore]
    public string PathToLFP { get => $"{PathToProjectRoot}/Project.lfp"; }

    [YamlIgnore]
    public int Hash { get; set; }
    [YamlIgnore]
    public int ProjectFileMaxHash = 0;

    [YamlIgnore]
    public bool IsSelected;
    [YamlIgnore]
    public bool IsUnsaved { get; set; } = false;

    [YamlIgnore]
    public ProjectViewerWindow Window { get; set; }
    [YamlIgnore]
    public ProjectCollection Parent { get; set; }
    [YamlIgnore]
    public List<LunaProjectFile> ProjectFiles { get; set; } = [];
    [YamlIgnore]
    public LunaProjectFile? CurrentProjectFile = null;

    /* This fucking line took 1 hour of my life for nothing.
     * YamlDotNet, please make your fucking Exceptions more precise. How the fuck was I supposed to know that
     * "(Line: 1, Col: 1, Idx: 0) - (Line: 1, Col: 1, Idx: 0): Exception during deserialization"
     * means that I'm missing a fucking constructor????
     * Please.
     */
    public LunaForgeProject() : this(null, string.Empty) { }

    #region IO

    public void ResetVariables(NewProjWindow newProjWin)
    {
        AuthorName = newProjWin.Author;
        ProjectName = newProjWin.ProjectName;
        AllowPr = newProjWin.AllowPr;
        AllowScPr = newProjWin.AllowScPr;
    }

    /// <summary>
    /// Tries and generate the project folder structure and the configuration/project file.ini.<br/>
    /// Only use this for debugging since the editor only supports template copying at Release.
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

            // Create .lfp file
            Save();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }

    }

    public bool IsFileOpened(string path) => ProjectFiles.Any(x => x.FullFilePath == path);

    #endregion
    #region Serialization

    public bool Save()
    {
        try
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(this);
            using StreamWriter sw = new(PathToLFP);
            sw.Write(yaml);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public static LunaForgeProject CreateFromFile(string pathToFile)
    {
        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            using StreamReader sr = new(pathToFile);
            LunaForgeProject proj = deserializer.Deserialize<LunaForgeProject>(sr);
            proj.PathToProjectRoot = Path.GetDirectoryName(pathToFile);
            return proj;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    #endregion
    #region Definitions

    public async Task<bool> OpenDefinitionFile(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        LunaDefinition newDef = await LunaDefinition.CreateFromFile(this, filePath);
        newDef.AllocHash(ref ProjectFileMaxHash);
        ProjectFiles.Add(newDef);

        return true;
    }

    #endregion
}
