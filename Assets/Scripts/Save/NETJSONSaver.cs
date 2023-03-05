using Newtonsoft.Json;
using System.IO;

public class NETJSONSaver : ISaver
{
    private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        Converters =
        {
            new Vector3FromSurrogateNETJSONConverter(),
            new Vector3ToSurrogateNETJSONConverter()
        }
    };

    public void Save(object objToSave, string saveDirectory)
    {
        using (var writer = File.CreateText(saveDirectory))
        {
            writer.Write(JsonConvert.SerializeObject(objToSave, Formatting.Indented, _serializerSettings));
        }
    }

    public T Load<T>(string saveDirectory)
    {
        using (var reader = File.OpenText(saveDirectory))
        {
            return JsonConvert.DeserializeObject<T>(reader.ReadToEnd(), _serializerSettings);
        }
    }
}

