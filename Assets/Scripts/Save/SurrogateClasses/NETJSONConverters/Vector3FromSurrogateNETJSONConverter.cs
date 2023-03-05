using Newtonsoft.Json;
using System;
using UnityEngine;

public class Vector3FromSurrogateNETJSONConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType)
        => objectType == typeof(Vector3Surrogate);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var surrogate = serializer.Deserialize<Vector3Surrogate>(reader);
        var original = new Vector3(surrogate.X, surrogate.Y, surrogate.Z);

        return original;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        => throw new NotImplementedException($"{GetType().Name} can't write to Json");
}