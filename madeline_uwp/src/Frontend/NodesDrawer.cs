using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline
{
    internal class NodesDrawer
    {
        public const float NODE_WIDTH = 90f;
        public const float NODE_HEIGHT = 30f;
        public const float ROUNDING = 5f;

        private Viewport viewport;

        public NodesDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            float zoom = viewport.zoom;
            var size = new Size(NODE_WIDTH * zoom, NODE_HEIGHT * zoom);
            float rounding = ROUNDING * zoom;
            foreach ((int nodeId, Node node) in graph.nodes)
            {
                var rect = new Rect(viewport.Into(node.pos).ToPoint(), size);
                session.FillRoundedRectangle(rect, rounding, rounding, Color.FromArgb(255, 64, 64, 64));
                Color borderColor = nodeId == viewport.graph.active ? Colors.Yellow : Colors.Black;
                borderColor = nodeId == viewport.graph.selection ? Colors.Red : borderColor;
                session.DrawRoundedRectangle(rect, rounding, rounding, borderColor);

                Plugin plugin = graph.plugins.Get(node.plugin);
                ListSlice<int> inputs = graph.inputs.Get(nodeId);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 iPos = InputPos(node.pos, i, plugin.inputs);

                    if (graph.nodes.TryGet(inputs.Consume(), out Node srcNode))
                    {
                        Vector2 oPos = OutputPos(srcNode.pos);

                        float r = 25f;
                        Vector2 direction = iPos - oPos;
                        float l = direction.Length();
                        var mul = Vector2.Dot(direction / l, -Vector2.UnitY) * 0.5f + 0.5f;
                        mul = 1f - mul;
                        mul *= mul;
                        mul *= mul;
                        mul = 1f - mul;
                        r *= mul;
                        r = Math.Min(r, l / 4f);

                        bool rightward = iPos.X > oPos.X;
                        var circleOffset = new Vector2(rightward ? r : -r, 0f);
                        Vector2 c1 = oPos + circleOffset;
                        Vector2 c2 = iPos - circleOffset;
                        Vector2 cd = c2 - c1;
                        float n = (float)Math.Atan2(cd.Y, Math.Abs(cd.X));
                        float m = (float)Math.Acos(2 * r / cd.Length());
                        float theta = (float)Math.PI - n - m;

                        var path = new CanvasPathBuilder(session.Device);
                        path.BeginFigure(oPos);

                        float start = (float)(rightward ? Math.PI : 0f);
                        float angle = rightward ? theta : -theta;
                        path.AddArc(c1, r, r, start, -angle);

                        start = (float)(rightward ? 0f : Math.PI);
                        path.AddArc(c2, r, r, start - angle, angle);

                        path.EndFigure(CanvasFigureLoop.Open);
                        var geo = CanvasGeometry.CreatePath(path);
                        session.DrawGeometry(geo, Colors.White);
                    }

                    DrawNodeIO(session, iPos);
                }
                DrawNodeIO(session, OutputPos(node.pos));

                var format = new CanvasTextFormat() { FontSize = 18f * zoom, WordWrapping = CanvasWordWrapping.NoWrap };
                var layout = new CanvasTextLayout(session.Device, node.name, format, 0f, 0f);

                var offset = new Vector2(NODE_WIDTH + 15f, 0f);
                session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.White);
                offset.Y -= 25f;
                session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.Gray);

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
            return viewport.Into(origin) + offset * viewport.zoom;
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
            return viewport.Into(origin) + offset * viewport.zoom;
        }
    }
}
