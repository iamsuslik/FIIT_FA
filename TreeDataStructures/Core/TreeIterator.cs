using System.Collections;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

internal struct TreeIterator<TKey, TValue, TNode> : 
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
        where TNode : Node<TKey, TValue, TNode>
{
    // probably add something here
    private readonly TNode? _root;
    private TNode? _current;
    private readonly TraversalStrategy _strategy; // or make it template parameter?
    private bool _isStarted; 

    public TreeIterator(TNode? root, TraversalStrategy strategy)
    {
        _root = root;
        _strategy = strategy;
        _current = null;
        _isStarted = false;
    }

    public TreeEntry<TKey, TValue> Current
    {
        get
        {
            if(_current is null)
            {
                throw new InvalidOperationException();
            }
            return new TreeEntry<TKey, TValue>(_current.Key, _current.Value, GetCurrentDepth(_current));
        }
    }
    object IEnumerator.Current => Current;


    private int GetCurrentDepth(TNode? node)
    {
        int depth = 0;
        while (node?.Parent is not null)
        {
            depth++;
            node = node.Parent;
        }
        return depth;
    }
    public bool MoveNext()
    {
        if (_root == null)
        {
            return false;
        }

        if (!_isStarted)
        {
            _current = GetFirstNode(_root, _strategy);
            _isStarted = true;
            return _current != null;
        }

        _current = GetNextNode(_current!, _strategy);
        return _current != null;
    }
        
    private TNode? GetFirstNode(TNode root, TraversalStrategy strategy)
    {
        TNode curr = root;
        // InOrder (Л-К-П) и PostOrder (Л-П-К)
        if (strategy == TraversalStrategy.InOrder || strategy == TraversalStrategy.PostOrder)
        {
            while (curr.Left != null)
            {
                curr = curr.Left;
            }

            if (strategy == TraversalStrategy.PostOrder && curr.Right != null)
            {
                return GetFirstNode(curr.Right, strategy);
            }
            return curr;
        }

        // InOrderReverse (П-К-Л) и PostOrderReverse (П-Л-К)
        else if (strategy == TraversalStrategy.InOrderReverse || strategy == TraversalStrategy.PostOrderReverse)
        {
            while (curr.Right != null)
            {
                curr = curr.Right;
            }

            if (strategy == TraversalStrategy.PostOrderReverse && curr.Left != null)
            {
                return GetFirstNode(curr.Left, strategy);
            }
            return curr;
        }

        // PreOrder (К-Л-П) и PreOrderReverse (К-П-Л)
        else if (strategy == TraversalStrategy.PreOrder || strategy == TraversalStrategy.PreOrderReverse)
        {
            return root;
        }

        else
        {
            throw new InvalidOperationException("Unknown traversal strategy");
        }
    }
        

    private TNode? GetNextNode(TNode node, TraversalStrategy strategy)
    {
        if (strategy == TraversalStrategy.InOrder)
        {
            if (node.Right != null)
            {
                TNode res = node.Right;
                while (res.Left != null)
                {
                    res = res.Left;
                }
                return res;
            }

            TNode? p = node.Parent;
            while (p != null && node == p.Right)
            {
                node = p;
                p = p.Parent;
            }

            return p;
        }

        else if (strategy == TraversalStrategy.InOrderReverse)
        {
            if (node.Left != null)
            {
                TNode res = node.Left;
                while (res.Right != null)
                {
                    res = res.Right;
                }
                return res;
            }

            TNode? pr = node.Parent;
            while (pr != null && node == pr.Left)
            {
                node = pr;
                pr = pr.Parent;
            }

            return pr;
        }

        else if (strategy == TraversalStrategy.PreOrder)
        {
            if (node.Left != null) {
                return node.Left;
            }
            if (node.Right != null) {
                return node.Right;
            }

            TNode? cur = node;
            while (cur.Parent != null)
            {
                if (cur == cur.Parent.Left && cur.Parent.Right != null)
                {
                    return cur.Parent.Right;
                }
                cur = cur.Parent;
            }

            return null;
        }

        else if (strategy == TraversalStrategy.PreOrderReverse)
        {
            if (node.Right != null)
            {
                return node.Right;
            }
            if (node.Left != null)
            {
                return node.Left;
            }
            TNode? c = node;
            while (c.Parent != null)
            {
                if (c == c.Parent.Right && c.Parent.Left != null)
                {
                    return c.Parent.Left;
                }
                c = c.Parent;
            }
            return null;
        }

        else if (strategy == TraversalStrategy.PostOrder)
        {
            TNode? parent = node.Parent;
            if (parent == null) {
                return null;
             }
            if (node == parent.Right || parent.Right == null)
            {
                return parent;
            }
            TNode resPost = parent.Right;
            while (resPost.Left != null || resPost.Right != null)
            {
                if (resPost.Left != null)
                {
                    resPost = resPost.Left;
                }
                else if (resPost.Right != null)
                {
                    resPost = resPost.Right;
                }
            }
            return resPost;
        }

        else if (strategy == TraversalStrategy.PostOrderReverse)
        {
            TNode? par = node.Parent;
            if (par == null)
            {
                return null;
            }
            if (node == par.Left || par.Left == null)
            {
                return par;
            }
            TNode resPreRev = par.Left;
            while (resPreRev.Right != null || resPreRev.Left != null)
            {
                if (resPreRev.Right != null)
                {
                    resPreRev = resPreRev.Right;
                }
                else if (resPreRev.Left != null)
                {
                    resPreRev = resPreRev.Left;
                }
            }
            return resPreRev;
        }

        else
        {
            throw new InvalidOperationException("Unknown traversal strategy");
        }
        
    }
    public void Reset()
    {
        _current = null;
        _isStarted = false;
    }

        
    public void Dispose()
    {
    }
    public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
