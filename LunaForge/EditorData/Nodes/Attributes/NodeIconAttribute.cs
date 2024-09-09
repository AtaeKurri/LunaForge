using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class NodeIconAttribute(string imagePath) : Attribute
{
    private readonly string path = imagePath;

    [DefaultValue("Unknown")]
    public virtual string Path { get => path; }
}
