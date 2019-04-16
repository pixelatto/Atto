
public interface IDataBaseService
{
	void Save<T>(string id, T value);
    T Load<T>(string id, T defaultValue = default(T)) where T : new();
    bool HasEntry(string id);
}