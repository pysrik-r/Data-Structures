using System;
using System.Collections.Generic;

class HashTable<TKey, TValue>
{
    private List<KeyValuePair<TKey, TValue>>[] table;
    private int size;
    private int count;
    private double loadFactor;

    public HashTable(int size, double loadFactor)
    {
        this.size = size;
        this.loadFactor = loadFactor;
        table = new List<KeyValuePair<TKey, TValue>>[size];
        count = 0;
    }

    private int GetIndex(TKey key)
    {
        int hash = key.GetHashCode();
        return Math.Abs(hash) % size;
    }

    // ADD / INSERT
    public void Add(TKey key, TValue value)
    {
        if ((double)count / size >= loadFactor)
        {
            Resize();
        }

        int index = GetIndex(key);

        if (table[index] == null)
            table[index] = new List<KeyValuePair<TKey, TValue>>();

        foreach (var kv in table[index])
        {
            if (kv.Key.Equals(key))
                throw new Exception("Key already exists");
        }

        table[index].Add(new KeyValuePair<TKey, TValue>(key, value));
        count++;
    }

    // GET
    public TValue Get(TKey key)
    {
        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kv in table[index])
            {
                if (kv.Key.Equals(key))
                    return kv.Value;
            }
        }

        throw new KeyNotFoundException("Key not found");
    }

    // UPDATE
    public void Update(TKey key, TValue newValue)
    {
        int index = GetIndex(key);

        if (table[index] != null)
        {
            for (int i = 0; i < table[index].Count; i++)
            {
                if (table[index][i].Key.Equals(key))
                {
                    table[index][i] = new KeyValuePair<TKey, TValue>(key, newValue);
                    return;
                }
            }
        }

        throw new KeyNotFoundException("Key not found");
    }

    // DELETE
    public bool Remove(TKey key)
    {
        int index = GetIndex(key);

        if (table[index] != null)
        {
            for (int i = 0; i < table[index].Count; i++)
            {
                if (table[index][i].Key.Equals(key))
                {
                    table[index].RemoveAt(i);
                    count--;
                    return true;
                }
            }
        }

        return false;
    }

    // CONTAINS
    public bool ContainsKey(TKey key)
    {
        int index = GetIndex(key);

        if (table[index] != null)
        {
            foreach (var kv in table[index])
            {
                if (kv.Key.Equals(key))
                    return true;
            }
        }

        return false;
    }

    // RESIZE
    private void Resize()
    {
        Console.WriteLine("Resizing...");

        int newSize = size * 2;
        var newTable = new List<KeyValuePair<TKey, TValue>>[newSize];

        foreach (var bucket in table)
        {
            if (bucket != null)
            {
                foreach (var kv in bucket)
                {
                    int newIndex = Math.Abs(kv.Key.GetHashCode()) % newSize;

                    if (newTable[newIndex] == null)
                        newTable[newIndex] = new List<KeyValuePair<TKey, TValue>>();

                    newTable[newIndex].Add(kv);
                }
            }
        }

        table = newTable;
        size = newSize;
    }

    public void Display()
    {
        for (int i = 0; i < size; i++)
        {
            Console.Write(i + ": ");
            if (table[i] != null)
            {
                foreach (var kv in table[i])
                {
                    Console.Write($"[{kv.Key}:{kv.Value}] -> ");
                }
                Console.WriteLine("null");
            }
            else
            {
                Console.WriteLine("empty");
            }
        }
    }
}