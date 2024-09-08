﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class NodeAttributeAttribute : Attribute
{
    public string Default { get; set; }

    public NodeAttributeAttribute(string defaultValue = "")
    {
        Default = defaultValue;
    }
}
