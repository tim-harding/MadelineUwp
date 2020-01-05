using System.Numerics;

namespace Madeline.Backend
{
    internal class Graph
    {
        public Table<Plugin> plugins = new Table<Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public RangeTable<int> inputs = new RangeTable<int>();

        public int active;

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
}
