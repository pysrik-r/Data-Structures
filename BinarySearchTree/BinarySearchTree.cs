using System;
using System.Collections;
using System.Collections.Generic;

public class BinarySearchTree<T> where T : IComparable<T>
{
    private int nodeCount = 0;
    private Node root = null;

    private class Node
    {
        public T data;
        public Node left, right;

        public Node(T data, Node left = null, Node right = null)
        {
            this.data = data;
            this.left = left;
            this.right = right;
        }
    }

    public bool IsEmpty() => nodeCount == 0;

    public int Size() => nodeCount;

    // Add
    public bool Add(T elem)
    {
        if (Contains(elem)) return false;
        root = Add(root, elem);
        nodeCount++;
        return true;
    }

    private Node Add(Node node, T elem)
    {
        if (node == null) return new Node(elem);

        if (elem.CompareTo(node.data) < 0)
            node.left = Add(node.left, elem);
        else
            node.right = Add(node.right, elem);

        return node;
    }

    // Remove
    public bool Remove(T elem)
    {
        if (!Contains(elem)) return false;
        root = Remove(root, elem);
        nodeCount--;
        return true;
    }

    private Node Remove(Node node, T elem)
    {
        if (node == null) return null;

        int cmp = elem.CompareTo(node.data);

        if (cmp < 0)
            node.left = Remove(node.left, elem);
        else if (cmp > 0)
            node.right = Remove(node.right, elem);
        else
        {
            // Case 1: no left child
            if (node.left == null) return node.right;

            // Case 2: no right child
            if (node.right == null) return node.left;

            // Case 3: two children
            Node tmp = FindMin(node.right);
            node.data = tmp.data;
            node.right = Remove(node.right, tmp.data);
        }

        return node;
    }

    private Node FindMin(Node node)
    {
        while (node.left != null)
            node = node.left;
        return node;
    }

    private Node FindMax(Node node)
    {
        while (node.right != null)
            node = node.right;
        return node;
    }

    // Contains
    public bool Contains(T elem) => Contains(root, elem);

    private bool Contains(Node node, T elem)
    {
        if (node == null) return false;

        int cmp = elem.CompareTo(node.data);

        if (cmp < 0) return Contains(node.left, elem);
        if (cmp > 0) return Contains(node.right, elem);

        return true;
    }

    // Height
    public int Height() => Height(root);

    private int Height(Node node)
    {
        if (node == null) return 0;
        return Math.Max(Height(node.left), Height(node.right)) + 1;
    }

    // Traversals using IEnumerable

    public IEnumerable<T> PreOrder()
    {
        return PreOrder(root);
    }

    private IEnumerable<T> PreOrder(Node node)
    {
        if (node == null) yield break;

        yield return node.data;
        foreach (var val in PreOrder(node.left)) yield return val;
        foreach (var val in PreOrder(node.right)) yield return val;
    }

    public IEnumerable<T> InOrder()
    {
        return InOrder(root);
    }

    private IEnumerable<T> InOrder(Node node)
    {
        if (node == null) yield break;

        foreach (var val in InOrder(node.left)) yield return val;
        yield return node.data;
        foreach (var val in InOrder(node.right)) yield return val;
    }

    public IEnumerable<T> PostOrder()
    {
        return PostOrder(root);
    }

    private IEnumerable<T> PostOrder(Node node)
    {
        if (node == null) yield break;

        foreach (var val in PostOrder(node.left)) yield return val;
        foreach (var val in PostOrder(node.right)) yield return val;
        yield return node.data;
    }

    public IEnumerable<T> LevelOrder()
    {
        if (root == null) yield break;

        Queue<Node> queue = new Queue<Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node.data;

            if (node.left != null) queue.Enqueue(node.left);
            if (node.right != null) queue.Enqueue(node.right);
        }
    }
}