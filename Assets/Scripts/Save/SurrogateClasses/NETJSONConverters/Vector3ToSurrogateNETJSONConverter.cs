using Newtonsoft.Json;
using System;
using UnityEngine;

public class Vector3ToSurrogateNETJSONConverter : JsonConverter
{
    public override bool CanRead => false;

    public override bool CanConvert(Type objectType)
        => objectType == typeof(Vector3);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        => throw new NotImplementedException($"{GetType().Name} can't read Json");

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var original = (Vector3)value;
        var surrogate = new Vector3Surrogate
        {
            X = original.x,
            Y = original.y,
            Z = original.z
        };

        serializer.Serialize(writer, surrogate);
    }
}

