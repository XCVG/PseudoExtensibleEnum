using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PseudoExtensibleEnum
{
    public class PEnumConverter : JsonConverter
    {
        private Type BaseEnumType;
        private bool IgnoreCase = true; //TODO allow configuration

        public PEnumConverter()
        {

        }

        public PEnumConverter(Type baseEnumType)
        {
            BaseEnumType = baseEnumType;
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

            var type = BaseEnumType ?? objectType;

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
            writer.WriteToken(JsonToken.Integer, value);
        }
    }
}