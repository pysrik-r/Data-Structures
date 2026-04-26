using System;

public class FenwickTreeRangeQueryPointUpdate
{
    // Size of the Fenwick Tree
    private readonly int N;

    // Internal tree array
    private long[] tree;

    // Constructor: empty tree of size sz (1-based indexing)
    public FenwickTreeRangeQueryPointUpdate(int sz)
    {
        if (sz < 0) throw new ArgumentException("Size cannot be negative!");
        N = sz + 1;
        tree = new long[N];
    }

    // Constructor: build Fenwick Tree from initial values (1-based array)
    public FenwickTreeRangeQueryPointUpdate(long[] values)
    {
        if (values == null) throw new ArgumentException("Values array cannot be null!");

        N = values.Length;
        values[0] = 0L;

        tree = (long[])values.Clone();

        for (int i = 1; i < N; i++)
        {
            int parent = i + Lsb(i);
            if (parent < N)
                tree[parent] += tree[i];
        }
    }

    // Least Significant Bit (LSB)
    private static int Lsb(int i)
    {
        return i & -i;
    }

    // Prefix sum from [1, i]
    private long PrefixSum(int i)
    {
        long sum = 0L;
        while (i != 0)
        {
            sum += tree[i];
            i &= ~Lsb(i); // same as i -= Lsb(i)
        }
        return sum;
    }

    // Range sum [left, right]
    public long Sum(int left, int right)
    {
        if (right < left)
            throw new ArgumentException("Make sure right >= left");

        if (left < 1 || right >= N)
            throw new IndexOutOfRangeException($"Index out of range [1, {N - 1}]");

        return PrefixSum(right) - PrefixSum(left - 1);
    }

    // Get value at index i
    public long Get(int i)
    {
        return Sum(i, i);
    }

    // Add value v at index i
    public void Add(int i, long v)
    {
        while (i < N)
        {
            tree[i] += v;
            i += Lsb(i);
        }
    }

    // Set index i to value v
    public void Set(int i, long v)
    {
        Add(i, v - Sum(i, i));
    }

    // Size of Fenwick Tree
    public int Size()
    {
        return N - 1;
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", tree) + "]";
    }
}
