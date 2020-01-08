using Madeline.Backend;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class SampleData
    {
        public static Graph DefaultGraph()
        {
            var graph = new Graph();

            graph.plugins.Insert(new Plugin("load", 0, new Plugin.ColorScheme(Palette.Gray4, Palette.White)));
            graph.plugins.Insert(new Plugin("merge", 2, new Plugin.ColorScheme(Palette.Green3, Palette.Green2)));
            graph.plugins.Insert(new Plugin("shuffle", 1, new Plugin.ColorScheme(Palette.Indigo3, Palette.Indigo2)));

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
