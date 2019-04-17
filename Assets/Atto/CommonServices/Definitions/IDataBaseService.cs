using RSG;

public interface IDataBaseService
{
    void WriteEntry<T>(string entryKey, T value);
    IPromise<T> ReadEntry<T>(string entryKey, T defaultEntryValue = default(T));
    void DeleteEntry<T>(string entryKey);
    IPromise<bool> HasEntry(string entryKey);
}