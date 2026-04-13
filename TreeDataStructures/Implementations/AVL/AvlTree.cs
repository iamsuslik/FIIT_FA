using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);
    
    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        AvlNode<TKey, TValue>? current = newNode.Parent;
        while (current != null)
        {
            UpdateHeight(current);
            Balance(current);
            current = current.Parent;
        }
    }

    private void Balance(AvlNode<TKey, TValue> node)
    {
        int bf = GetHeight(node.Right) - GetHeight(node.Left);
        if (bf == 2)
        {
            if (GetHeight(node.Right!.Right) >= GetHeight(node.Right.Left))
            {
                RotateLeft(node);
            }
            else
            {
                RotateBigLeft(node);
            }
        }
        else if (bf == -2)
        {
            if (GetHeight(node.Left!.Left) >= GetHeight(node.Left.Right))
            {
                RotateRight(node);
            }
            else
            {
                RotateBigRight(node);
            }
        }
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child)
    {
        AvlNode<TKey, TValue>? current = parent;
        while (current != null)
        {
            UpdateHeight(current);
            Balance(current);
            current = current.Parent;
        }
    }
}

