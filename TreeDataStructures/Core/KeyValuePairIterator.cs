using System.Collections;

using TreeDataStructures.Interfaces;

  

namespace TreeDataStructures.Core;

  

internal struct KeyValuePairIterator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>

{
    private readonly IEnumerator<TreeEntry<TKey, TValue>> _innerIterator;

  

    public KeyValuePairIterator(IEnumerable<TreeEntry<TKey, TValue>> source)
    {
      _innerIterator = source.GetEnumerator();

    }

  

    public KeyValuePair<TKey, TValue> Current

    {
        get
        {
            var entry = _innerIterator.Current;
            return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }

    }

  

    object IEnumerator.Current => Current;

    public bool MoveNext() => _innerIterator.MoveNext();

    public void Reset() => _innerIterator.Reset();

    public void Dispose() => _innerIterator.Dispose();

}
