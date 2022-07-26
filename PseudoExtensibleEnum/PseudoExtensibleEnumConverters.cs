using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PseudoExtensibleEnum
{
    public class PxEnumConverter : JsonConverter
    {
        private Type BaseEnumType;
        private bool IgnoreCase = true;

        public PxEnumConverter()
        {

        }

        public PxEnumConverter(Type baseEnumType)
        {
            BaseEnumType = baseEnumType;
        }

        public PxEnumConverter(Type baseEnumType, bool ignoreCase)
        {
            BaseEnumType = baseEnumType;
            IgnoreCase = ignoreCase;
        }

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string sValue = reader.Value as string;
            long? lValue = reader.Value as long?;

            Type nullableObjectType = Nullable.GetUnderlyingType(objectType);

            if (sValue == null && lValue == null)
            {
                if (objectType.IsValueType && nullableObjectType == null)
                    throw new JsonSerializationException($"Error converting value {{null}} to type '{objectType.Name}'. Path '{reader.Path}'");

                return null;
            }

            var type = BaseEnumType ?? nullableObjectType ?? objectType;

            Func<object> parseFunc;
            if(sValue != null)
            {
                parseFunc = () => PxEnum.Parse(type, sValue, IgnoreCase);
            }
            else
            {
                Type backingType = BaseEnumType != null ? Enum.GetUnderlyingType(BaseEnumType) : (nullableObjectType ?? objectType);
                parseFunc = () => Convert.ChangeType(lValue.Value, backingType);
            }

            if(nullableObjectType != null)
            {
                if (nullableObjectType.IsEnum)
                    return Enum.ToObject(nullableObjectType, parseFunc());
                return Convert.ChangeType(parseFunc(), nullableObjectType);
            }

            if (objectType.IsEnum)
                return Enum.ToObject(objectType, parseFunc());

            return Convert.ChangeType(parseFunc(), objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteToken(JsonToken.Integer, value);
        }
    }

    public class PxEnumArrayConverter : JsonConverter
    {
        private Type BaseEnumType;
        private bool IgnoreCase = true;

        public PxEnumArrayConverter()
        {

        }

        public PxEnumArrayConverter(Type baseEnumType)
        {
            BaseEnumType = baseEnumType;
        }

        public PxEnumArrayConverter(Type baseEnumType, bool ignoreCase)
        {
            BaseEnumType = baseEnumType;
            IgnoreCase = ignoreCase;
        }

        public override bool CanConvert(Type objectType)
        {
            if(objectType.IsArray)
            {
                var baseType = objectType.GetElementType();
                if (baseType.IsEnum || PxEnumConverterUtils.IsIntegralType(baseType))
                    return true;
            }
            else
            {
                if(objectType.IsGenericType)
                {
                    if(objectType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>) || t.GetGenericTypeDefinition() == typeof(IReadOnlyList<>) || t.GetGenericTypeDefinition() == typeof(ISet<>)))                    
                        return true;
                    
                }
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type collectionType, object existingValue, JsonSerializer serializer)
        {
            Type objectType = typeof(object);
            Type collectionBaseType = objectType;
            if(collectionType.IsGenericType)
            {
                objectType = collectionType.GetGenericArguments()[0];
                collectionBaseType = collectionType.GetGenericTypeDefinition();
            }

            Type nullableObjectType = Nullable.GetUnderlyingType(objectType);
            Type baseObjectType = nullableObjectType ?? objectType;

            var enumType = BaseEnumType ?? nullableObjectType ?? objectType;

            if (reader.TokenType != JsonToken.StartArray)
            {
                throw new JsonSerializationException(""); //TODO details
            }
            reader.Read();

            List<object> rawItems = new List<object>();            
            while(reader.TokenType != JsonToken.EndArray)
            {                
                rawItems.Add(reader.Value);
                reader.Read();
            }

            List<object> parsedItems = new List<object>();
            foreach(var rawItem in rawItems)
            {
                if(rawItem == null)
                {
                    parsedItems.Add(null);
                    continue;
                }

                //check if numeric or string, try to convert with appropriate path
                if(rawItem is string s)
                {
                    object enumValue = PxEnum.Parse(enumType, s);
                    if(objectType.IsEnum)
                    {
                        enumValue = Enum.ToObject(objectType, enumValue);
                    }
                    else
                    {
                        enumValue = Convert.ChangeType(enumValue, objectType);
                    }
                    parsedItems.Add(enumValue);
                }
                else if(PxEnumConverterUtils.IsIntegralType(rawItem.GetType()))
                {
                    object enumValue;
                    if (objectType.IsEnum)
                    {
                        enumValue = Enum.ToObject(objectType, rawItem);
                    }
                    else
                    {
                        enumValue = Convert.ChangeType(rawItem, objectType);
                    }
                    parsedItems.Add(enumValue);
                }
                else
                {
                    throw new JsonSerializationException(""); //TODO details
                }
            }

            if(collectionBaseType.IsArray)
            {
                var array = Array.CreateInstance(collectionBaseType.GetElementType(), parsedItems.Count);
                for (int i = 0; i < array.Length; i++)
                {
                    array.SetValue(parsedItems[i], i);
                }
                return array;
            }
            else if(collectionBaseType == typeof(IList<>) || collectionBaseType == typeof(IReadOnlyList<>) || collectionBaseType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IList<>) || t.GetGenericTypeDefinition() == typeof(IReadOnlyList<>)))
            {
                var constructedListType = typeof(List<>).MakeGenericType(objectType);
                IList list = (IList)Activator.CreateInstance(constructedListType);
                for(int i = 0; i < parsedItems.Count; i++)
                {
                    list.Add(parsedItems[i]);
                }
                return list;
            }
            else if(collectionBaseType == typeof(ISet<>) || collectionBaseType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ISet<>)))
            {
                var constructedSetType = typeof(HashSet<>).MakeGenericType(objectType);
                var addMethod = constructedSetType.GetMethod("Add");
                object set = Activator.CreateInstance(constructedSetType);                
                for (int i = 0; i < parsedItems.Count; i++)
                {
                    addMethod.Invoke(set, new object[] { parsedItems[i] });
                }
                return set;
            }

            throw new JsonSerializationException(""); //TODO details
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            var enumerable = value as IEnumerable;
            foreach(object item in enumerable)
            {
                writer.WriteToken(JsonToken.Integer, item);
            }
            writer.WriteEndArray();
        }
    }

    public class PxEnumObjectConverter : JsonConverter
    {
        private Type BaseEnumType;
        private bool IgnoreCase = true;

        public PxEnumObjectConverter()
        {

        }

        public PxEnumObjectConverter(Type baseEnumType)
        {
            BaseEnumType = baseEnumType;
        }

        public PxEnumObjectConverter(Type baseEnumType, bool ignoreCase)
        {
            BaseEnumType = baseEnumType;
            IgnoreCase = ignoreCase;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    internal static class PxEnumConverterUtils
    {
        public static bool IsIntegralType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return true;
                default:
                    return false;
            }
        }
    }
}