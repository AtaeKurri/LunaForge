using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes.Attributes;

/// <summary>
/// Identify a <see cref="TreeNode"/> ignore validation if being tested. 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IgnoreValidationAttribute : Attribute
{
}
