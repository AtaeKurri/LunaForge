﻿using LunaForge.EditorData.Project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace LunaForge;

public struct DefaultConfig
{
    public DefaultConfig() { }

    [DefaultValue(false)]
    public bool DefinitionsWindowOpen { get; set; } = false;

    [DefaultValue("John Dough")]
    public string AuthorName { get; set; } = "John Dough";

    [DefaultValue("")]
    public string LastUsedPath { get; set; } = "";

    public Dictionary<string, bool> EnabledPlugins { get; set; } = [];
}

public static class Configuration
{
    private static string PathToConfig => Path.Combine(Directory.GetCurrentDirectory(), "Config.yaml");

    public static DefaultConfig Default;
    

    public static bool Save()
    {
        try
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(Default);
            using FileStream fs = new(PathToConfig, FileMode.Create, FileAccess.Write);
            using StreamWriter sw = new(fs);
            sw.Write(yaml);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public static void Load()
    {
        try
        {
            if (!File.Exists(PathToConfig))
            {
                Default = new DefaultConfig();
                Save();
                return;
            }

            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            using StreamReader sr = new(PathToConfig);
            DefaultConfig conf = deserializer.Deserialize<DefaultConfig>(sr);
            Default = conf;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return;
        }
    }
}
