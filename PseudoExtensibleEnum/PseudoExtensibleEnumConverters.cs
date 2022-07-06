using Newtonsoft.Json;
using System;
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

            Type nullableObjectType = Nullable.GetUnderlyingType(objectType);

            if (sValue == null)
            {
                if (objectType.IsValueType && nullableObjectType == null)
                    throw new JsonSerializationException($"Error converting value {{null}} to type '{objectType.Name}'. Path '{reader.Path}'");

                return null;
            }

            var type = BaseEnumType ?? nullableObjectType ?? objectType;

            if(nullableObjectType != null)
            {
                if (nullableObjectType.IsEnum)
                    return Enum.ToObject(nullableObjectType, PxEnum.Parse(type, sValue, IgnoreCase));
                return Convert.ChangeType(PxEnum.Parse(type, sValue, IgnoreCase), nullableObjectType);
            }

            if (objectType.IsEnum)
                return Enum.ToObject(objectType, PxEnum.Parse(type, sValue, IgnoreCase));

            return Convert.ChangeType(PxEnum.Parse(type, sValue, IgnoreCase), objectType);
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
}