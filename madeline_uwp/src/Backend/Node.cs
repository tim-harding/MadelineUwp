using System.Numerics;

namespace Madeline.Backend
{
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
}
