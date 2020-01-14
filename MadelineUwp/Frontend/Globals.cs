using Madeline.Backend;
using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend
{
    internal static class Globals
    {
        public static History history;
        public static NodeGraph graph;
        public static CanvasDrawingSession session;

        public static void Init()
        {
            graph = new NodeGraph();
            history = new History(graph);
        }
    }
}
