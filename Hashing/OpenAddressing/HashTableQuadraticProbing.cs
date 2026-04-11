using System;
using System.Collections.Generic;

public class HashTableQuadraticProbing<K, V>
{
    private const int DEFAULT_CAPACITY = 8;
    private const double DEFAULT_LOAD_FACTOR = 0.5;

    private double loadFactor;
    private int capacity, threshold;

    private int keyCount = 0;       // Number of ACTIVE keys
    private int usedBuckets = 0;    // Includes tombstones
    private int modificationCount = 0;

    private K[] keys;
    private V[] values;
    private bool[] isTombstone;     // Marks deleted slots

    /// <summary>
    /// Constructor: Initializes hash table with given capacity and load factor.
    /// Ensures capacity is always a power of two.
    /// </summary>
    public HashTableQuadraticProbing(int capacity = DEFAULT_CAPACITY, double loadFactor = DEFAULT_LOAD_FACTOR)
    {
        this.loadFactor = loadFactor;
        this.capacity = NextPowerOfTwo(capacity);
        this.threshold = (int)(this.capacity * loadFactor);

        keys = new K[this.capacity];
        values = new V[this.capacity];
        isTombstone = new bool[this.capacity];
    }

    /// <summary>
    /// Computes the next power of two greater than or equal to n.
    /// Required for quadratic probing to work correctly.
    /// </summary>
    private int NextPowerOfTwo(int n)
    {
        int power = 1;
        while (power < n) power <<= 1;
        return power;
    }

    /// <summary>
    /// Converts a hash code into a valid index using bitmasking.
    /// 
    /// WHY use '&' instead of '%':
    /// - Modulo (%) is relatively expensive (division operation)
    /// - Bitwise AND (&) is much faster (single CPU instruction)
    /// 
    /// This ONLY works because capacity is always a power of 2.
    /// 
    /// Example:
    /// capacity = 16 → binary: 10000
    /// mask = capacity - 1 = 15 → binary: 01111
    /// 
    /// Suppose hash = 29 → binary: 11101
    /// 
    /// Using %:
    /// 29 % 16 = 13
    /// 
    /// Using &:
    /// 11101
    /// & 01111
    /// -------
    ///   01101 = 13
    /// 
    /// Same result, but faster.
    /// </summary>
    private int NormalizeIndex(int hash)
    {
        return (hash & 0x7FFFFFFF) & (capacity - 1);
    }

    /// <summary>
    /// Quadratic probing function:
    /// P(x) = (x^2 + x) / 2
    /// Determines how far to jump when resolving collisions.
    /// </summary>
    private int Probe(int x)
    {
        return (x * x + x) / 2;
    }

    /// <summary>
    /// Resizes the hash table when load factor threshold is exceeded.
    /// Doubles capacity and rehashes all ACTIVE elements.
    /// Tombstones are discarded during this process.
    /// </summary>
    private void Resize()
    {
        capacity *= 2;
        threshold = (int)(capacity * loadFactor);

        var oldKeys = keys;
        var oldValues = values;
        var oldTombstone = isTombstone;

        keys = new K[capacity];
        values = new V[capacity];
        isTombstone = new bool[capacity];

        keyCount = 0;
        usedBuckets = 0;

        // Reinsert valid elements into new table
        for (int i = 0; i < oldKeys.Length; i++)
        {
            if (oldKeys[i] != null && !oldTombstone[i])
            {
                Put(oldKeys[i], oldValues[i]);
            }
        }
    }

    /// <summary>
    /// Inserts a new key-value pair or updates an existing key.
    /// Uses quadratic probing to resolve collisions.
    /// Implements "swap optimization" to reduce probe lengths.
    /// </summary>
    public V Put(K key, V value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));

        // Resize if too many used buckets (including tombstones)
        if (usedBuckets >= threshold) Resize();

        int hash = NormalizeIndex(key.GetHashCode());
        int i = hash;
        int j = -1; // First tombstone index
        int x = 1;

        while (true)
        {
            // Found a tombstone
            if (keys[i] != null && isTombstone[i])
            {
                if (j == -1) j = i;
            }
            // Found an occupied slot
            else if (keys[i] != null)
            {
                // Key already exists → update
                if (keys[i]!.Equals(key))
                {
                    V oldValue = values[i];

                    // Swap optimization
                    if (j != -1)
                    {
                        keys[j] = key;
                        values[j] = value;
                        isTombstone[j] = false;

                        isTombstone[i] = true;
                        values[i] = default!;
                    }
                    else
                    {
                        values[i] = value;
                    }

                    return oldValue;
                }
            }
            // Empty slot found
            else
            {
                int target = (j != -1) ? j : i;

                keys[target] = key;
                values[target] = value;
                isTombstone[target] = false;

                keyCount++;
                usedBuckets++;
                modificationCount++;

                return default!;
            }

            // Continue probing
            i = NormalizeIndex(hash + Probe(x++));
        }
    }

    /// <summary>
    /// Retrieves the value associated with a given key.
    /// Uses the same probing sequence as insertion.
    /// </summary>
    public V Get(K key)
    {
        if (key == null) return default!;

        int hash = NormalizeIndex(key.GetHashCode());
        int i = hash;
        int x = 1;

        while (keys[i] != null)
        {
            if (!isTombstone[i] && keys[i]!.Equals(key))
                return values[i];

            i = NormalizeIndex(hash + Probe(x++));
        }

        return default!;
    }

    /// <summary>
    /// Removes a key from the hash table.
    /// Marks the slot as a tombstone instead of clearing it.
    /// This preserves the probe chain.
    /// </summary>
    public V Remove(K key)
    {
        if (key == null) return default!;

        int hash = NormalizeIndex(key.GetHashCode());
        int i = hash;
        int x = 1;

        while (keys[i] != null)
        {
            if (!isTombstone[i] && keys[i]!.Equals(key))
            {
                V oldValue = values[i];

                isTombstone[i] = true;
                values[i] = default!;

                keyCount--;
                modificationCount++;

                return oldValue;
            }

            i = NormalizeIndex(hash + Probe(x++));
        }

        return default!;
    }

    /// <summary>
    /// Checks if a key exists in the table.
    /// </summary>
    public bool ContainsKey(K key)
    {
        return !EqualityComparer<V>.Default.Equals(Get(key), default!);
    }

    /// <summary>
    /// Returns number of active keys.
    /// </summary>
    public int Size() => keyCount;

    /// <summary>
    /// Returns true if table is empty.
    /// </summary>
    public bool IsEmpty() => keyCount == 0;

    /// <summary>
    /// Clears the entire hash table.
    /// </summary>
    public void Clear()
    {
        Array.Clear(keys, 0, capacity);
        Array.Clear(values, 0, capacity);
        Array.Clear(isTombstone, 0, capacity);

        keyCount = usedBuckets = 0;
        modificationCount++;
    }

    /// <summary>
    /// Returns a list of all active keys.
    /// Skips null and tombstone entries.
    /// </summary>
    public List<K> Keys()
    {
        var list = new List<K>();
        for (int i = 0; i < capacity; i++)
        {
            if (keys[i] != null && !isTombstone[i])
                list.Add(keys[i]);
        }
        return list;
    }

    /// <summary>
    /// Returns a list of all active values.
    /// </summary>
    public List<V> Values()
    {
        var list = new List<V>();
        for (int i = 0; i < capacity; i++)
        {
            if (keys[i] != null && !isTombstone[i])
                list.Add(values[i]);
        }
        return list;
    }
}