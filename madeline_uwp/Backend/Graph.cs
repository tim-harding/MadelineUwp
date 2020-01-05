using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Madeline.Backend
{
    internal struct Plugin
    {
        public string name;
        public int inputs;

        public Plugin(string name, int inputs)
        {
            this.name = name;
            this.inputs = inputs;
        }
    }

    internal struct Node
    {
        public Vector2 pos;
        public string name;
        public int plugin;

        public Node(Vector2 pos, Plugin plugin, int pluginId)
        {
            this.pos = pos;
            this.plugin = pluginId;
            name = plugin.name;
        }
    }

    internal class Graph
    {
        public Table<Plugin> plugins = new Table<Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public MultiTable<int> inputs = new MultiTable<int>();

        public void InsertNode(Vector2 pos, int pluginId)
        {
            if (plugins.TryGet(pluginId, out Plugin plugin))
            {
                nodes.Insert(new Node(pos, plugin, pluginId));
                inputs.Extend(plugin.inputs, -1);
            }
        }

        public void Connect(int output, int input, int slot)
        {
            inputs.Update(input, slot, output);
        }

        public void Disconnect(int input, int slot)
        {
            inputs.Update(input, slot, -1);
        }
    }

    internal class Table<T> : IEnumerable<(int, T)>
    {
        private int next;
        private List<int> ids = new List<int>();
        private List<T> values = new List<T>();

        public void Insert(T value)
        {
            int id = next++;
            ids.Add(id);
            values.Add(value);
        }

        public void Update(int id, T value)
        {
            int index = ids.BinarySearch(id);
            if (index >= 0)
            {
                values[index] = value;
            }
        }

        public void Delete(int id)
        {
            int index = ids.BinarySearch(id);
            if (index >= 0)
            {
                ids.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        public bool TryGet(int id, out T value)
        {
            int index = ids.BinarySearch(id);
            bool success = index >= 0;
            value = success ? values[index] : default;
            return success;
        }

        public T Get(int id)
        {
            int index = ids.BinarySearch(id);
            return values[index];
        }

        public IEnumerator<(int, T)> GetEnumerator()
        {
            for (int i = 0; i < ids.Count; i++)
            {
                yield return (ids[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal struct Range
    {
        public int start;
        public int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int Count()
        {
            return end - start;
        }
    }

    internal struct ListSlice<T> : IEnumerable<T>
    {
        private List<T> values;
        private Range range;

        public ListSlice(List<T> values, Range range)
        {
            this.values = values;
            this.range = range;
        }

        public T Consume()
        {
            return values[range.start++];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = range.start; i < range.end; i++)
            {
                yield return values[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class MultiTable<T>
    {
        private Table<Range> indices = new Table<Range>();
        private List<T> values = new List<T>();

        public void Extend(int count, T template)
        {
            Range range = new Range(values.Count, values.Count + count);
            indices.Insert(range);
            for (int i = 0; i < count; i++)
            {
                values.Add(template);
            }
        }

        public void Insert(IEnumerable<T> multiple)
        {
            int start = values.Count;
            values.AddRange(multiple);
            indices.Insert(new Range(start, values.Count));
        }

        public void Update(int id, int offset, T value)
        {
            if (indices.TryGet(id, out Range range) && offset < range.Count())
            {
                values[range.start + offset] = value;
            }
        }

        public void Delete(int id)
        {
            if (indices.TryGet(id, out Range range))
            {
                values.RemoveRange(range.start, range.Count());
                indices.Delete(id);
            }
        }

        public bool TryGet(int id, out ListSlice<T> slice)
        {
            if (indices.TryGet(id, out Range range))
            {
                slice = new ListSlice<T>(values, range);
                return true;
            }
            slice = new ListSlice<T>(values, new Range(0, 0));
            return false;
        }

        public ListSlice<T> Get(int id)
        {
            if (indices.TryGet(id, out Range range))
            {
                return new ListSlice<T>(values, range);
            }
            return new ListSlice<T>(values, new Range(0, 0));
        }
    }
}
