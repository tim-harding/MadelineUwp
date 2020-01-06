using System.Numerics;
using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class Graph
    {
        public Table<Plugin> plugins = new Table<Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public RangeTable<int> inputs = new RangeTable<int>();

        public int hover = -1;
        public int active = -1;
        public List<int> selection = new List<int>();

        public void InsertNode(Vector2 pos, int pluginId)
        {
            if (plugins.TryGet(pluginId, out Plugin plugin))
            {
                nodes.Insert(new Node(pos, plugin, pluginId));
                inputs.Extend(plugin.inputs, -1);
            }
        }

        public void DeleteNode(int id)
        {
            nodes.Delete(id);
            inputs.Delete(id);
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
}
