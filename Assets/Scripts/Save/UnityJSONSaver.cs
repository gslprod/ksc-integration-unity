using System.IO;
using UnityEngine;

public class UnityJSONSaver : ISaver
{
    public void Save(object objToSave, string saveDirectory)
    {
        using (var writer = File.CreateText(saveDirectory))
        {
            writer.Write(JsonUtility.ToJson(objToSave, true));
        }
    }

    public T Load<T>(string saveDirectory)
    {
        using (var reader = File.OpenText(saveDirectory))
        {
            return JsonUtility.FromJson<T>(reader.ReadToEnd());
        }
    }
}
