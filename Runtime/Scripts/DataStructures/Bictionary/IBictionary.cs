using System.Collections.Generic;

public interface IBictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{
    Dictionary<TKey, TValue> KeyMap { get; }
    Dictionary<TValue, TKey> ValueMap { get; }

    void Add(TKey key, TValue value);
    bool RemoveKey(TKey key);
    bool RemoveValue(TValue value);

    void Clear();
    bool ContainsKey(TKey key);
    bool ContainsValue(TValue value);

    int Count();

    new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();
    IEnumerator<KeyValuePair<TValue, TKey>> GetValueEnumerator();
}