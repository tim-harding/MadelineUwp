using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Madeline.Backend
{
    internal struct Plugin
    {
        public string name;
        public int inputs;
    }

    internal struct Node
    {
        public Vector2 pos;
        public string name;
        public int plugin;
    }

    internal class Graph
    {
        public Table<Plugin> plugins = new Table<Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public Table<List<int>> inputs = new Table<List<int>>();

        public void InsertNode(Vector2 pos, int pluginId)
        {
            if (!plugins.TryGet(pluginId, out Plugin plugin))
            {
                return;
            }

            Node node = new Node
            {
                pos = pos,
                plugin = pluginId,
                name = plugin.name,
            };

            nodes.Insert(node);

            List<int> inputs = new List<int>(plugin.inputs);
            for (int i = 0; i < plugin.inputs; i++)
            {
                inputs.Add(-1);
            }

            this.inputs.Insert(inputs);
        }

        public void Connect(int output, int input, int slot)
        {
            if (inputs.TryGet(input, out List<int> slots))
            {
                slots[slot] = output;
            }
        }

        public void Disconnect(int input, int slot)
        {
            if (inputs.TryGet(input, out List<int> slots))
            {
                slots[slot] = -1;
            }
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
}
