using Madeline.Backend;
using Madeline.Frontend.Drawing;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class SampleData
    {
        public static void Init(Viewport viewport)
        {
            Graph graph = viewport.graph;

            graph.plugins.Add("load", new Plugin("load", 0, new Plugin.ColorScheme(Palette.Gray4, Palette.White)));
            graph.plugins.Add("merge", new Plugin("merge", 2, new Plugin.ColorScheme(Palette.Green3, Palette.Green2)));
            graph.plugins.Add("shuffle", new Plugin("shuffle", 1, new Plugin.ColorScheme(Palette.Indigo3, Palette.Indigo2)));

            History history = viewport.history;
            history.SubmitChange(new HistoricEvents.InsertNode(graph, graph.plugins["load"], new Vector2(200f, 200f)));
            history.SubmitChange(new HistoricEvents.InsertNode(graph, graph.plugins["load"], new Vector2(400f, 200f)));
            history.SubmitChange(new HistoricEvents.InsertNode(graph, graph.plugins["merge"], new Vector2(300f, 300f)));
            history.SubmitChange(new HistoricEvents.InsertNode(graph, graph.plugins["shuffle"], new Vector2(300f, 400f)));

            history.SubmitChange(new HistoricEvents.Connect(0, 2, 0));
            history.SubmitChange(new HistoricEvents.Connect(1, 2, 1));
            history.SubmitChange(new HistoricEvents.Connect(2, 3, 0));
        }
    }
}
