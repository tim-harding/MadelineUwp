using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline
{
    internal class NodesDrawer
    {
        public const float NODE_WIDTH = 90f;
        public const float NODE_HEIGHT = 30f;
        public const float ROUNDING = 10f;

        private Viewport viewport;

        public NodesDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            foreach ((int nodeId, Node node) in graph.nodes)
            {
                var size = new Size(NODE_WIDTH, NODE_HEIGHT);
                var rect = new Rect(viewport.Into(node.pos).ToPoint(), size);
                session.FillRoundedRectangle(rect, ROUNDING, ROUNDING, Color.FromArgb(255, 64, 64, 64));
                Color borderColor = nodeId == viewport.graph.active ? Colors.Yellow : Colors.Black;
                session.DrawRoundedRectangle(rect, ROUNDING, ROUNDING, borderColor);

                Plugin plugin = graph.plugins.Get(node.plugin);
                ListSlice<int> inputs = graph.inputs.Get(nodeId);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 inPos = InputPos(node.pos, i, plugin.inputs);
                    int inputNodeId = inputs.Consume();
                    if (graph.nodes.TryGet(inputNodeId, out Node srcNode))
                    {
                        Vector2 outPos = OutputPos(srcNode.pos);
                        session.DrawLine(inPos, outPos, Colors.White);
                    }
                    DrawNodeIO(session, inPos);
                }
                DrawNodeIO(session, OutputPos(node.pos));

                var offset = new Vector2(NODE_WIDTH + 15f, 0f);
                session.DrawText(node.name, viewport.Into(node.pos + offset), Colors.White);
                offset.Y -= 25f;
                session.DrawText(plugin.name, viewport.Into(node.pos + offset), Colors.Gray);
            }
        }

        private Vector2 InputPos(Vector2 origin, int input, int inputs)
        {
            const float NODE_SEPARATION = 35f;
            float local = input - (inputs - 1) / 2f;
            var offset = new Vector2
            {
                X = NODE_WIDTH / 2f + local * NODE_SEPARATION,
                Y = 0f,
            };
            return viewport.Into(origin) + offset;
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center)
        {
            session.FillCircle(center, 5, Colors.LightGray);
            session.DrawCircle(center, 5, Colors.Black);
        }

        private Vector2 OutputPos(Vector2 origin)
        {
            var offset = new Vector2
            {
                X = NODE_WIDTH / 2f,
                Y = NODE_HEIGHT,
            };
            return viewport.Into(origin) + offset;
        }
    }
}
