using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.API.Services;

/// <summary>
/// This attribute indicates whether an applicable service should be injected into the plugin.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class PluginServiceAttribute : Attribute
{
}
