using System.Data.Common;
using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    private RbColor _lastRemovedNodeColor;
    protected override void RemoveNode(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue> physicallyRemovedNode;
    
        if (node.Left != null && node.Right != null)
        {
            physicallyRemovedNode = node.Right;
            while (physicallyRemovedNode.Left != null) 
            {
                physicallyRemovedNode = physicallyRemovedNode.Left;
            }
        }
        else
        {
            physicallyRemovedNode = node;
        }

        _lastRemovedNodeColor = physicallyRemovedNode.Color;

        base.RemoveNode(node);
    }
    private RbNode<TKey, TValue>? GetUncle(RbNode<TKey, TValue> node)
    {
        var parent = node.Parent;
        var grandparent = parent?.Parent;

        if (grandparent == null) return null;

        if (parent == grandparent.Left)
        {
            return grandparent.Right;
        }
        else
        {
            return grandparent.Left;
        }
    }

    private bool IsBlack(RbNode<TKey, TValue>? node)
    {
        return node == null || node.Color == RbColor.Black;
    }

    private void BalanceAfterDeletion(RbNode<TKey, TValue>? node, RbNode<TKey, TValue> parent)
    {
        while (node != Root && IsBlack(node))
        {
            if (node == parent.Right)
            {
                RbNode<TKey, TValue>? sibling = parent.Left;

                // Случай 1: брат красный
                if (sibling != null && sibling.Color == RbColor.Red)
                {
                    sibling.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateRight(parent);
                    sibling = parent.Left; 
                }
                // ЧЧ6 Случай 2: брат черный и оба его ребенка черные
                else if (sibling != null && IsBlack(sibling.Left) && IsBlack(sibling.Right))
                {
                    sibling.Color = RbColor.Red;
                    node = parent;
                    parent = node.Parent!;
                }
                
                else 
                {
                    // ЧЧ5 Случай 3: левый сын чёрный с правым красным внуком
                    if (sibling != null && IsBlack(sibling.Left))
                    {
                        sibling.Right!.Color = RbColor.Black;
                        sibling.Color = RbColor.Red;
                        RotateLeft(sibling);
                        sibling = parent.Left;
                    }
                    // ЧЧ4 Случай 4: брат черный и его левый сын красный
                    sibling?.Color = parent.Color;
                    parent.Color = RbColor.Black;
                    if (sibling?.Left != null)
                    {
                        sibling.Left.Color = RbColor.Black;
                    }
                    RotateRight(parent);
                    node = Root!;
                }
                
            }

            else // node == parent.Left
            {
                RbNode<TKey, TValue>? sibling = parent.Right;

                // Случай 1: брат красный
                if (sibling != null && sibling.Color == RbColor.Red)
                {
                    sibling.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateLeft(parent);
                    sibling = parent.Right; 
                }
                // ЧЧ6 Случай 2: брат черный и оба его ребенка черные
                else if (sibling != null && IsBlack(sibling.Right) && IsBlack(sibling.Left))
                {
                    sibling.Color = RbColor.Red;
                    node = parent;
                    parent = node.Parent!;
                }
                else 
                {
                    // ЧЧ5 Случай 3: левый сын чёрный с правым красным внуком
                    if (sibling != null && IsBlack(sibling.Right))
                    {
                        sibling.Left!.Color = RbColor.Black;
                        sibling.Color = RbColor.Red;
                        RotateRight(sibling);
                        sibling = parent.Right;
                    }
                    // ЧЧ4 Случай 4: брат черный и его левый сын красный
                    sibling?.Color = parent.Color;
                    parent.Color = RbColor.Black;
                    if (sibling?.Right != null)
                    {
                        sibling.Right.Color = RbColor.Black;
                    }
                    RotateLeft(parent);
                    node = Root!;
                }
            }
        }
        if (node != null) 
        {
            node.Color = RbColor.Black;
        }
    }

    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new RbNode<TKey, TValue>(key, value);
    }
    
    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        if (newNode.Parent == null)
        {
            newNode.Color = RbColor.Black;
            return;
        }
        if (newNode.Parent.Color == RbColor.Black)
        {
            return;
        }
        var parent = newNode.Parent;
        var grandparent = parent.Parent;
        if (grandparent == null)
        {
            parent.Color = RbColor.Black;
            return;
        }

        var uncle = GetUncle(newNode);

        if (uncle != null && uncle.Color == RbColor.Red)
        {
            newNode.Parent!.Color = RbColor.Black;
            uncle.Color = RbColor.Black;
            grandparent.Color = RbColor.Red;
            OnNodeAdded(grandparent);
        }
        else
        {
            if (parent == grandparent.Left)
            {
                if (newNode == parent.Left)
                {
                    RotateRight(grandparent);
                    parent.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                }
                else
                {
                    RotateBigRight(grandparent);
                    newNode.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                }
            }
            else
            {
                if (newNode == parent.Right)
                {
                    RotateLeft(grandparent);
                    parent.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                }
                else
                {
                    RotateBigLeft(grandparent);
                    newNode.Color = RbColor.Black;
                    grandparent.Color = RbColor.Red;
                }
            }
        }
    }
    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child)
    {
        if (_lastRemovedNodeColor == RbColor.Red)
        {
            return;
        }
        if (child != null && child.Color == RbColor.Red)
        {
            child.Color = RbColor.Black;
        }
        else
        {
           if (parent != null)
            {
                BalanceAfterDeletion(child, parent);
            }
        }

        if (Root != null)
        {
            Root.Color = RbColor.Black;
        }
    }
}
