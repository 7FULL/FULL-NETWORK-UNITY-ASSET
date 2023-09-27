using System;
using System.Collections.Generic;

public class SyncList<T>
{
    private List<T> list = new List<T>();

    public void Add(T item)
    {
        list.Add(item);
        // TODO: Send the RPC to the server to add the item
    }

    public void Remove(T item)
    {
        if (list.Remove(item))
        {
            // TODO: Send the RPC to the server to remove the item
        }
    }
    
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        // TODO: Send the RPC to the server to remove the item
    }

    public List<T> GetAll()
    {
        return list;
    }
    
    public T Get(int index)
    {
        return list[index];
    }

    public void Set(int index, T item)
    {
        list[index] = item;
        // TODO: Send the RPC to the server to set the item
    }
    
    public int Count
    {
        get { return list.Count; }
    }

    public void Clear()
    {
        list.Clear();
        // TODO: Send the RPC to the server to clear the list
    }

    public void AddRange(IEnumerable<T> collection)
    {
        list.AddRange(collection);
        // TODO: Send the RPC to the server to add the range
    }

    public void RemoveRange(int index, int count)
    {
        list.RemoveRange(index, count);
        // TODO: Send the RPC to the server to remove the range
    }

    public void Insert(int index, T item)
    {
        list.Insert(index, item);
        // TODO: Send the RPC to the server to insert the item
    }
}