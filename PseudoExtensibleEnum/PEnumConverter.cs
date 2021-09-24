using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string sValue = reader.ReadAsString();

        if(sValue == null)
        {
            //TODO check serializer settings?
            return existingValue;
        }

        var type = BaseEnumType ?? objectType;
        if (objectType.IsEnum)
            return PEnum.ParseToEnum(type, sValue, IgnoreCase);

        return PEnum.ParseToValue(type, sValue, IgnoreCase);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteToken(JsonToken.Integer, value);
    }
}

public class PEnumContainerConverter : JsonConverter<PEnum>
{
    public override PEnum ReadJson(JsonReader reader, Type objectType, PEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, PEnum value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        if (serializer.TypeNameHandling != TypeNameHandling.None)
        {
            writer.WritePropertyName("$type");
            writer.WriteValue(string.Format("{0}, {1}", value.GetType().ToString(), value.GetType().Assembly.GetName().Name));
        }
        writer.WritePropertyName("$value");
        writer.WriteValue(value.Value);

        writer.WriteEndObject();
    }
}