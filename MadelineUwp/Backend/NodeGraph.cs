using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class NodeGraph
    {
        public Dictionary<string, Plugin> plugins = new Dictionary<string, Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public int viewing = -1;
        public int active = -1;
    }
}
