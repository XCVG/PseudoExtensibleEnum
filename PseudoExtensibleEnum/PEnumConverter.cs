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
            string sValue = reader.ReadAsString();

            if (sValue == null)
            {
                //TODO check serializer settings?
                return existingValue;
            }

            var type = BaseEnumType ?? objectType;
            if (objectType.IsEnum)
                return PxEnum.Parse(type, sValue, IgnoreCase);

            return PxEnum.Parse(type, sValue, IgnoreCase);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteToken(JsonToken.Integer, value);
        }
    }
}