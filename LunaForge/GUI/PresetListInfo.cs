using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.GUI;

public class PresetListInfo
{
    public string DirName { get; set; } = string.Empty;
    public Dictionary<string, string> PresetList { get; set; } = [];

    public PresetListInfo(string dirPath)
    {
        DirName = new DirectoryInfo(dirPath).Name;
        foreach (string preset in Directory.GetFiles(dirPath, "*.lfdpreset"))
        {
            PresetList.Add(preset, Path.GetFileNameWithoutExtension(preset));
        }
    }
}
