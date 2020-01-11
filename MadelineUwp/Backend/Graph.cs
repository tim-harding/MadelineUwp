namespace Madeline.Backend
{
    internal class Graph
    {
        public Table<Plugin> plugins = new Table<Plugin>();
        public Table<Node> nodes = new Table<Node>();
        public RangeTable<int> inputs = new RangeTable<int>();
    }
}
