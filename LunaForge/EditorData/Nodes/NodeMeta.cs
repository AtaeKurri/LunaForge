using LunaForge.EditorData.Nodes.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunaForge.EditorData.Nodes;

public sealed class NodeMeta
{
    public bool IsFolder { get; }

    public bool IsDefinition { get; }

    public bool IsLeafNode { get; }

    public bool CannotBeDeleted { get; }

    public bool CannotBeBanned { get; }

    public string Icon { get; }

    public Type[] RequireParent { get; } = null;
    public Type[][] RequireAncestor { get; } = null;

    public int? CreateInvokeId { get; } = null;

    public NodeMeta(TreeNode node)
    {
        Type type = node.GetType();

        IsFolder = type.IsDefined(typeof(IsFolderAttribute), false);
        IsDefinition = type.IsDefined(typeof(DefinitionNodeAttribute), true);
        IsLeafNode = type.IsDefined(typeof(LeafNodeAttribute), true);
        CannotBeDeleted = type.IsDefined(typeof(CannotBeDeletedAttribute), false);
        CannotBeBanned = type.IsDefined(typeof(CannotBeBannedAttribute), false);

        string pathToImage = type.GetAttributeValue((NodeIconAttribute img) => img.Path);
        Icon = $"{(string.IsNullOrEmpty(pathToImage) ? "Unknown" : pathToImage)}";
    }

    public static Type[] GetTypes(Type[] src)
    {
        if (src != null)
        {
            LinkedList<Type> types = new LinkedList<Type>();
            Type it = typeof(IEnumerable<Type>);
            foreach (Type t in src)
            {
                if (it.IsAssignableFrom(t))
                {
                    IEnumerable<Type> o = t.GetConstructor(Type.EmptyTypes).Invoke([]) as IEnumerable<Type>;
                    foreach (Type ty in o)
                    {
                        types.AddLast(ty);
                    }
                }
                else
                {
                    types.AddLast(t);
                }
            }
            return types.ToArray();
        }
        return null;
    }
}

public static class AttributeExtensions
{
    public static TValue GetAttributeValue<TAttribute, TValue>(
        this Type type,
        Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
    {
        try
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }
        catch { return default(TValue); }
    }
}