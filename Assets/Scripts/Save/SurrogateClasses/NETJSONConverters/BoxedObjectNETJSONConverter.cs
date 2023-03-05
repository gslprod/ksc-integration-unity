using NamedPipeConnection;
using Newtonsoft.Json;
using System;

public class BoxedObjectNETJSONConverter : JsonConverter<BoxedObjectWrapper>
{
    public override BoxedObjectWrapper ReadJson(JsonReader reader, Type objectType, BoxedObjectWrapper existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var surrogate = serializer.Deserialize<BoxedObjectWrapperSurrogate>(reader);
        if (surrogate == null)
            return null;

        var boxedObj = JsonConvert.DeserializeObject(surrogate.SerializedObject, Type.GetType(surrogate.TypeName, true, false));
        return new BoxedObjectWrapper(boxedObj!);
    }

    public override void WriteJson(JsonWriter writer, BoxedObjectWrapper value, JsonSerializer serializer)
    {
        var serializedObject = JsonConvert.SerializeObject(value.BoxedObject);
        var surrogate = new BoxedObjectWrapperSurrogate(serializedObject, value.BoxedObject.GetType().FullName);

        serializer.Serialize(writer, surrogate);
    }
}

