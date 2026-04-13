using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;


public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null) 
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;
    public IComparer<TKey> Comparer { get; protected set; } = comparer ?? Comparer<TKey>.Default; // use it to compare Keys

    public int Count { get; protected set; }
    
    public bool IsReadOnly => false;

    public ICollection<TKey> Keys 
    {
        get 
        {
            var keys = new List<TKey>(Count);
            foreach (var entry in InOrder()) keys.Add(entry.Key);
            return keys;
        }
    }

    public ICollection<TValue> Values 
    {
        get 
        {
            var values = new List<TValue>(Count);
            foreach (var entry in InOrder()) values.Add(entry.Value);
            return values;
        }
    }
    
    
    public virtual void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (Root == null)
        {
            Root = CreateNode(key, value);
            Count = 1;
            OnNodeAdded(Root);
            return;
        }

        TNode? current = Root;
        TNode? parent = null;
        bool goleft = false;

        while (current != null)
        {
            parent = current;
            int cmpr = Comparer.Compare(key, current.Key);

            if (cmpr == 0)
            {
                current.Value = value;
                return;
            }

            else if (cmpr < 0)
            {
                goleft = true;
                current = current.Left;
            }

            else
            {
                goleft = false;
                current = current!.Right;
            }
        }

        TNode newNode = CreateNode(key, value);
        newNode.Parent = parent;

        if (goleft)
        {
            parent!.Left = newNode;
        }
        else
        {
            parent!.Right = newNode;
        }

        Count++;
        OnNodeAdded(newNode);

    }

    
    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null) { return false; }

        RemoveNode(node);
        Count--;
        return true;
    }
    
    
    protected virtual void RemoveNode(TNode node)
    {
        TNode? parent = node.Parent;
        TNode? child;

        if (node.Left == null)
        {
            child = node.Right;
            Transplant(node, node.Right);
        }
        else if (node.Right == null)
        {
            child = node.Left;
            Transplant(node, node.Left);
        }

        else
        {
            TNode minRight = node.Right;
            while (minRight.Left != null)
            {
                minRight = minRight.Left;
            }
            child = minRight;

            if (minRight.Parent != node)
            {
                Transplant(minRight, minRight.Right);

                minRight.Right = node.Right;
                if (minRight.Right != null)
                {
                    minRight.Right.Parent = minRight;
                }
            }

            Transplant(node, minRight);

            minRight.Left = node.Left;
            minRight.Left.Parent = minRight;
        }

        OnNodeRemoved(parent, child);
    }

    public virtual bool ContainsKey(TKey key) => FindNode(key) != null;
    
    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }
        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }

    
    #region Hooks
    
    /// <summary>
    /// Вызывается после успешной вставки
    /// </summary>
    /// <param name="newNode">Узел, который встал на место</param>
    protected virtual void OnNodeAdded(TNode newNode) { }
    
    /// <summary>
    /// Вызывается после удаления. 
    /// </summary>
    /// <param name="parent">Узел, чей ребенок изменился</param>
    /// <param name="child">Узел, который встал на место удаленного</param>
    protected virtual void OnNodeRemoved(TNode? parent, TNode? child) { }
    
    #endregion
    
    
    #region Helpers
    protected abstract TNode CreateNode(TKey key, TValue value);
    
    protected int GetHeight(TNode? node) => node?.Height ?? 0;

    protected void UpdateHeight(TNode node)
    {
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
    }
    
    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0) { return current; }
            current = cmp < 0 ? current.Left : current.Right;
        }
        return null;
    }

    protected void RotateLeft(TNode x)
    {
        if (x.Right == null) return;
        TNode y = x.Right;
        x.Right = y.Left;
        if (y.Left != null)
        {
            y.Left.Parent = x;
        }

        y.Parent = x.Parent;
        if (x.Parent == null)
        {
            Root = y;
        }
        else if (x.IsLeftChild)
        {
            x.Parent.Left = y;
        }
        else
        {
            x.Parent.Right = y;
        }

        y.Left = x;
        x.Parent = y;

        UpdateHeight(x); 
        UpdateHeight(y);
    }

    protected void RotateRight(TNode y)
    {
        if (y.Left == null) return;
        TNode x = y.Left;

        y.Left = x.Right;

        if (x.Right != null)
        {
            x.Right.Parent = y;
        }
        x.Parent = y.Parent;

        if (y.Parent == null)
        {
            Root = x;
        }
        else if (y.IsLeftChild)
        {
            y.Parent.Left = x;
        }
        else
        {
            y.Parent.Right = x;
        }

        x.Right = y;
        y.Parent = x;

        UpdateHeight(y);
        UpdateHeight(x);
    }
    
    protected void RotateBigLeft(TNode x)
    {
        if (x.Right?.Left == null) return;
        RotateRight(x.Right);
        RotateLeft(x);
    }
    
    protected void RotateBigRight(TNode y)
    {
        if (y.Left?.Right == null) return;
        RotateLeft(y.Left);
        RotateRight(y);
    }
    
    protected void RotateDoubleLeft(TNode x)
    {
        var y = x.Right;
        if (y == null) return;
        RotateLeft(x);
        RotateLeft(y);
    }
    
    protected void RotateDoubleRight(TNode y)
    {
        var x = y.Left;
        if (x == null) return;
        RotateRight(y);
        RotateRight(x);
    }
    
    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }
        if (v != null)
        {
            v.Parent = u.Parent;
        }
    }
    #endregion

    
    
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrder() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.InOrder);
    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrder() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.PreOrder);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrder() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.PostOrder);
    public IEnumerable<TreeEntry<TKey, TValue>>  InOrderReverse() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.InOrderReverse);
    public IEnumerable<TreeEntry<TKey, TValue>>  PreOrderReverse() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.PreOrderReverse);
    public IEnumerable<TreeEntry<TKey, TValue>>  PostOrderReverse() => new TreeIterator<TKey, TValue, TNode>(Root, TraversalStrategy.PostOrderReverse);
    
    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
   
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new KeyValuePairIterator<TKey, TValue>(InOrder());
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();



    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public void Clear() { Root = null; Count = 0; }
    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array), "The target array cannot be null.");
        }
        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex), "The starting index cannot be negative.");
        }
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("The destination array does not have enough space to copy the tree elements.");
        }
        foreach (var entry in InOrder())
        {
            array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            arrayIndex++;
        }

    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}



