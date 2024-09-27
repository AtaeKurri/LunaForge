using LunaForge.EditorData.Nodes;
using LunaForge.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LunaForge.EditorData.Project;

public enum DefinitionMetaType
{
    Boss, BossBG, Bullet, Laser, BentLaser,
    Object, Background, Enemy, Task, Item, Player, PlayerBullet,
    BGM, Image, ImageGroup, SE, Animation, Particle, Texture, FX, Font, TTF
}

public struct CachedDefinition
{
    public string PathToDefinition;
    public string[] Parameters;
    public string[] AccessibleFrom;
}

[Serializable]
public class DefinitionsCache
{
    #region Serialized Properties

    public List<CachedDefinition> Definitions = [];

    #endregion

    [YamlIgnore]
    public LunaForgeProject ParentProj { get; set; }

    [YamlIgnore]
    public string PathToCache { get; set; }

    public DefinitionsCache() { }

    #region IO

    public bool Save()
    {
        try
        {
            ISerializer serializer = new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .Build();

            string yaml = serializer.Serialize(this);
            using StreamWriter sw = new(PathToCache);
            sw.Write(yaml);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public static DefinitionsCache LoadFromProject(LunaForgeProject parentProj)
    {
        try
        {
            IDeserializer deserializer = new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            string pathToCache = Path.Combine(parentProj.PathToData, "defcache.yaml");
            using FileStream fs = new(pathToCache, FileMode.OpenOrCreate, FileAccess.Read);
            using StreamReader sr = new(fs);
            DefinitionsCache cache = deserializer.Deserialize<DefinitionsCache>(sr) ?? new();
            cache.ParentProj = parentProj;
            cache.PathToCache = pathToCache;
            return cache;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return null;
        }
    }

    #endregion
    #region Caching

    public void UpdateCache(LunaDefinition loadedDefinition)
    {
        TreeNode defNode = loadedDefinition.TreeNodes[0];
        CachedDefinition cached = new()
        {
            PathToDefinition = loadedDefinition.FileName,
            //Parameters = defNode.Attributes
        };
        Definitions.Add(cached);
        Save();
    }

    public void RemoveFromCache(LunaDefinition loadedDefinition)
    {
        Definitions.RemoveAll(x => x.PathToDefinition == loadedDefinition.FileName);
        Save();
    }

    #endregion
}
