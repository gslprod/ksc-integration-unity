
public interface ISaver
{
    void Save(object objToSave, string savePath);

    T Load<T>(string savePath);
}
