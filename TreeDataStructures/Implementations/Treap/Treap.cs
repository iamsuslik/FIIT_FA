using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Разрезает дерево с корнем <paramref name="root"/> на два поддерева:
    /// Left: все ключи <= <paramref name="key"/>
    /// Right: все ключи > <paramref name="key"/>
    /// </summary>
    
    
    protected virtual (TreapNode<TKey, TValue>? Left, TreapNode<TKey, TValue>? Right) Split(TreapNode<TKey, TValue>? root, TKey key)
    {
        if (root == null) return (null, null);
        if (Comparer.Compare(root.Key, key) <= 0)
        {
            var (leftSubtree, rightSubtree) = Split(root.Right, key);
            root.Right = leftSubtree;
            if (leftSubtree != null)
            {
                leftSubtree.Parent = root;
            }
            if (rightSubtree != null)
            {
                rightSubtree.Parent = null;
            }
            return (root, rightSubtree);
        }
        else
        {
            var (leftSubtree, rightSubtree) = Split(root.Left, key);
            root.Left = rightSubtree;
            if (rightSubtree != null) 
            {
                rightSubtree.Parent = root;
            }
            if (leftSubtree != null)
            {
                leftSubtree.Parent = null;
            }
            return (leftSubtree, root);
        }
    }

    /// <summary>
    /// Сливает два дерева в одно.
    /// Важное условие: все ключи в <paramref name="left"/> должны быть меньше ключей в <paramref name="right"/>.
    /// Слияние происходит на основе Priority (куча).
    /// </summary>
    protected virtual TreapNode<TKey, TValue>? Merge(TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right)
    {
        if (left == null) return right;
        if (right == null) return left;

        if (left.Priority > right.Priority)
        {
            left.Right = Merge(left.Right, right);
            if (left.Right != null) left.Right.Parent = left;
            return left;
        }
        else
        {
            right.Left = Merge(left, right.Left);
            if (right.Left != null) right.Left.Parent = right;
            return right;
        }
    }
    

    public override void Add(TKey key, TValue value)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        var existingNode = FindNode(key);
        if (existingNode != null)
        {
            existingNode.Value = value;
            return;
        }
        var (l, r) = Split(Root, key);
        TreapNode<TKey, TValue> newNode = CreateNode(key, value);
        Root = Merge(Merge(l, newNode), r);
        if (Root != null)
        {
            Root.Parent = null;
        }
        Count++;
    }

    public override bool Remove(TKey key)
    {
        var nodeToRemove = (TreapNode<TKey, TValue>?)FindNode(key);
        if (nodeToRemove == null) return false;

        var mergedSubtree = Merge(nodeToRemove.Left, nodeToRemove.Right);

        if (nodeToRemove.Parent == null)
        {
            Root = mergedSubtree;
        }
        else if (nodeToRemove.IsLeftChild)
        {
            nodeToRemove.Parent.Left = mergedSubtree;
        }
        else
        {
            nodeToRemove.Parent.Right = mergedSubtree;
        }

        if (mergedSubtree != null)
        {
            mergedSubtree.Parent = nodeToRemove.Parent;
        }
        nodeToRemove.Left = null;
        nodeToRemove.Right = null;
        nodeToRemove.Parent = null;

        Count--;
        return true;
    }

    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }
    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode)
    {
      
    }
    
    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child)
    {
    
    }
    
}
