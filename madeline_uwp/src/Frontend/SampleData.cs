using Madeline.Backend;
using System.Numerics;

namespace Madeline
{
    internal class SampleData
    {
        public static Graph DefaultGraph()
        {
            var graph = new Graph();

            graph.plugins.Insert(new Plugin { inputs = 0, name = "load" });
            graph.plugins.Insert(new Plugin { inputs = 2, name = "merge" });
            graph.plugins.Insert(new Plugin { inputs = 1, name = "shuffle" });

            graph.InsertNode(new Vector2(0, 0), 0);
            graph.InsertNode(new Vector2(200, 0), 0);
            graph.InsertNode(new Vector2(100, 100), 1);
            graph.InsertNode(new Vector2(100, 200), 2);

            graph.Connect(0, 2, 0);
            graph.Connect(1, 2, 1);
            graph.Connect(2, 3, 0);

            return graph;
        }
    }
}
