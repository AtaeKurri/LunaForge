using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Documents;

[Serializable]
public class ProjectConfiguration
{
    public string AuthorName { get; set; } = string.Empty;

    [JsonConstructor]
    public ProjectConfiguration() { }
}