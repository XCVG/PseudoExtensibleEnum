using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//no idea if ANY of this is correct but the theory:
// [PseudoExtensibleEnum] on the base enum
// [PseudoExtendEnum] on enums that extend the base enum
// actual places where you need to store values will be backing-type (usually or always int)
// but we can utilize this data in custom json converter etc
// will probably provide a convenience method like InterpretEnum(string val) that does magic lookup (or just parses number)
// custom converter will always store as number (since preserving nice name isn't critical and we can always round trip it) 

[AttributeUsage(AttributeTargets.Enum)]
public class PseudoExtensibleAttribute : Attribute
{

}

[AttributeUsage(AttributeTargets.Enum)]
public class PseudoExtendAttribute : Attribute
{
    public Type BaseType { get; private set; }

    public PseudoExtendAttribute(Type baseType)
    {
        BaseType = baseType;
    }
}

//base/util class, maybe base of container class
public class PEnum
{

    private static Dictionary<Type, Type[]> EnumExtensionCache;

    static PEnum()
    {
        RefreshCaches();
    }

    public static void RefreshCaches()
    {
        var allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes());

        RefreshCaches(allTypes);
    }

    public static void RefreshCaches(IEnumerable<Type> allTypes)
    {
        EnumExtensionCache = new Dictionary<Type, Type[]>();

        var baseTypes = allTypes
            .Where(t => t.IsEnum)
            .Where(t => t.GetCustomAttribute<PseudoExtensibleAttribute>() != null);

        foreach(var baseType in baseTypes)
        {
            var extendedTypes = allTypes
                .Where(t => t.IsEnum)
                .Where(t =>
                {
                    var attr = t.GetCustomAttribute<PseudoExtendAttribute>();
                    if (attr == null)
                        return false;
                    return attr.BaseType == baseType;
                })
                .ToArray();

            EnumExtensionCache.Add(baseType, extendedTypes);
        }
    }

    public static IEnumerable<Type> GetExtensionsForType(Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException($"Type {type?.Name} must be an enum", nameof(type));

        return EnumExtensionCache[type];
    }

    public static string Format(Type enumType, object value, string format)
    {
        throw new NotImplementedException();
    }

    public static string GetName(Type enumType, object value)
    {
        throw new NotImplementedException();
    }

    public static string[] GetNames(Type enumType)
    {
        throw new NotImplementedException();
    }

    public static Array GetValues(Type enumType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Parses a string or numeric-as-string value 
    /// </summary>
    public static object ParseToValue(Type type, string value, bool ignoreCase)
    {
        if (!type.IsEnum)
            throw new ArgumentException($"Type {type?.Name} must be an enum", nameof(type));

        var nType = Enum.GetUnderlyingType(type);

        if (long.TryParse(value, out long lValue))
        {            
            return Convert.ChangeType(lValue, nType);
            //TODO should this be checked against valid extensible values?
        }

        if (TryParseEnum(type, value, ignoreCase, out var bResult))
            return Convert.ChangeType(bResult, nType);

        foreach(var eType in GetExtensionsForType(type))
        {
            if (TryParseEnum(eType, value, ignoreCase, out var eResult))
                return Convert.ChangeType(Convert.ChangeType(eResult, Enum.GetUnderlyingType(eType)), nType);
        }

        throw new ArgumentException($"Value {value} is not valid for enum {type?.Name} or any of its pseudoextensions", nameof(value));
    }

    public static object ParseToEnum(Type type, string value, bool ignoreCase)
    {
        object rValue = ParseToValue(type, value, ignoreCase);
        //TODO should this be checked against valid extensible values?
        return Enum.ToObject(type, rValue);
    }

    //TODO generic and ParseToContainer

    private static bool TryParseEnum(Type type, string value, bool ignoreCase, out object result)
    {
        //this is a dumb way of doing it but should work
        try
        {
            result = Enum.Parse(type, value, ignoreCase);
            return true;
        }
        catch
        {

        }
        result = null;
        return false;
    }


    public object Value { get; set; }
}

//typed container class (?!)
public class PEnum<T> : PEnum where T : Enum
{
    public T TypedValue
    {
        get
        {
            return (T)Enum.ToObject(typeof(T), Convert.ChangeType(Value, Enum.GetUnderlyingType(typeof(T))));
        }
        set
        {
            Value = Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)));
        }
    }
}

