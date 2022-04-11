using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MBW.Utilities.IPAddresses.DataStructures;

public abstract class IPSet<TNetwork, TValue> where TNetwork : struct
{
    private readonly List<Node> _nodes;

    protected internal IPSet()
    {
        _nodes = new();
    }

    private (int index, Node node)? Find(TNetwork key)
    {
        (int index, Node node)? mostSpecific = null;
        int mostSpecificMask = 0;

        for (int i = 0; i < _nodes.Count; i++)
        {
            Node node = _nodes[i];
            if (!Contains(node.Key, key))
                continue;

            int nodeMask = Mask(node.Key);

            if (mostSpecific == null || nodeMask > mostSpecificMask)
            {
                mostSpecific = (i, node);
                mostSpecificMask = nodeMask;
            }
        }

        return mostSpecific;
    }

    public void Add(TNetwork key, TValue value)
    {
        (int index, IPSet<TNetwork, TValue>.Node node)? node = Find(key);

        if (!node.HasValue || Compare(node.Value.node.Key, key) != 0)
        {
            _nodes.Add(new Node(key, value));
        }
        else
        {
            // Replace
            _nodes[node.Value.index] = new Node(key, value);
        }
    }

    public bool TryGet(TNetwork key, out TNetwork? foundNetwork, out TValue? value)
    {
        (int index, IPSet<TNetwork, TValue>.Node node)? node = Find(key);

        if (!node.HasValue)
        {
            foundNetwork = default;
            value = default;
            return false;
        }

        foundNetwork = node.Value.node.Key;
        value = node.Value.node.Value;
        return true;
    }

    protected abstract int Mask(TNetwork network);
    protected abstract bool Contains(TNetwork network, TNetwork other);
    protected abstract int Compare(TNetwork network, TNetwork other);

    public IEnumerable<(TNetwork network, TValue value)> GetValues() => _nodes.Select(s => (s.Key, s.Value));
    public bool Remove(TNetwork key, bool matchExact = true)
    {
        bool wasRemoved = false;
        for (int i = _nodes.Count - 1; i >= 0; i--)
        {
            if (!Contains(key, _nodes[i].Key) || (matchExact && Compare(_nodes[i].Key, key) != 0))
                continue;

            _nodes.RemoveAt(i);
            wasRemoved = true;
        }

        return wasRemoved;
    }

    [DebuggerDisplay("Key: {Key}, value: {Value}")]
    class Node
    {
        public Node(TNetwork key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TNetwork Key { get; }
        public TValue Value { get; }
    }
}
