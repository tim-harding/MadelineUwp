using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class Graph
    {
        public Dictionary<string, Plugin> plugins = new Dictionary<string, Plugin>();
        public Table<Node> nodes = new Table<Node>();
    }
}
