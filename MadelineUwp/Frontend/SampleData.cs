using Madeline.Backend;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class SampleData
    {
        public static void Init()
        {
            NodeGraph graph = Globals.graph;

            graph.plugins.Add("load", new Plugin
            {
                name = "load",
                inputs = 0,
                colors = new Plugin.ColorScheme(Palette.Gray4, Palette.White),
                controls = new Control[] {
                    new TextControl("filename", ""),
                },
            });
            graph.plugins.Add("merge", new Plugin
            {
                name = "merge",
                inputs = 2,
                colors = new Plugin.ColorScheme(Palette.Green3, Palette.Green2),
                controls = new Control[] { },
            });
            graph.plugins.Add("shuffle", new Plugin
            {
                name = "shuffle",
                inputs = 1,
                colors = new Plugin.ColorScheme(Palette.Indigo3, Palette.Indigo2),
                controls = new Control[] {
                    new IntControl("r", 0),
                    new IntControl("g", 1),
                    new IntControl("b", 2),
                    new IntControl("a", 3),
                },
            });

            History history = Globals.history;
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
