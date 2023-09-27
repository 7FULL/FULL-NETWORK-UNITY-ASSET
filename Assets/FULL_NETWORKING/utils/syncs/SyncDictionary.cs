using System.Collections.Generic;

public class SyncDictionary<T> : Dictionary<T, T>
{
    private Dictionary<T, T> dictionary = new Dictionary<T, T>();

    public void Add(T key, T value)
    {
        dictionary.Add(key, value);
        // TODO: Send the RPC to the server to add the item
    }

    public void Remove(T key)
    {
        if (dictionary.Remove(key))
        {
            // TODO: Send the RPC to the server to remove the item
        }
    }
}