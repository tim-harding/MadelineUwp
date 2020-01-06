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
            foreach ((int id, Node value) node in graph.nodes)
            {
                DrawNodeBody(node, session);
                Plugin plugin = graph.plugins.Get(node.value.plugin);
                ListSlice<int> inputs = graph.inputs.Get(node.id);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 iPos = InputPos(node.value.pos, i, plugin.inputs);
                    if (graph.nodes.TryGet(inputs.Consume(), out Node upstream))
                    {
                        Vector2 oPos = OutputPos(upstream.pos);
                        DrawWire(session, iPos, oPos);
                    }
                    DrawNodeIO(session, iPos);
                }
                DrawNodeIO(session, OutputPos(node.value.pos));
                DrawNodeLabel(session, node.value);
            }
        }

        private void DrawNodeBody((int id, Node value) node, CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            float zoom = viewport.zoom;
            float rounding = ROUNDING * zoom;
            var size = new Size(NODE_WIDTH * zoom, NODE_HEIGHT * zoom);
            var upperLeft = viewport.Into(node.value.pos).ToPoint();
            var rect = new Rect(upperLeft, size);
            Color borderColor = node.id == graph.hover ? Colors.Yellow : Colors.Black;
            borderColor = node.id == graph.active ? Colors.Red : borderColor;
            session.FillRoundedRectangle(rect, rounding, rounding, Color.FromArgb(255, 64, 64, 64));
            session.DrawRoundedRectangle(rect, rounding, rounding, borderColor);
        }

        private void DrawWire(CanvasDrawingSession session, Vector2 iPos, Vector2 oPos)
        {
            float r = 25f;
            Vector2 dir = iPos - oPos;
            float len = dir.Length();
            float mul = -(dir / len).Y * 0.5f + 0.5f;
            mul = 1f - mul;
            mul *= mul;
            mul *= mul;
            mul *= mul;
            mul = 1f - mul;
            r *= mul;
            r = Math.Min(r, len / 4f);

            float rightward = Convert.ToInt32(iPos.X > oPos.X);
            float sign = rightward * 2f - 1f;
            var circleOffset = new Vector2(r * sign, 0f);
            Vector2 c1 = oPos + circleOffset;
            Vector2 c2 = iPos - circleOffset;
            Vector2 cd = c2 - c1;
            float n = (float)Math.Atan2(cd.Y, Math.Abs(cd.X));
            float m = (float)Math.Acos(2f * r / cd.Length());
            float pi = (float)Math.PI;
            float theta = pi - n - m;
            theta *= sign;

            var path = new CanvasPathBuilder(session.Device);
            path.BeginFigure(oPos);

            float start = rightward * pi;
            path.AddArc(c1, r, r, start, -theta);
            start = pi - start;
            path.AddArc(c2, r, r, start - theta, theta);

            path.EndFigure(CanvasFigureLoop.Open);
            var geo = CanvasGeometry.CreatePath(path);
            session.DrawGeometry(geo, Colors.White);
        }

        private void DrawNodeLabel(CanvasDrawingSession session, Node node)
        {
            var format = new CanvasTextFormat()
            {
                FontSize = 18f * viewport.zoom,
                WordWrapping = CanvasWordWrapping.NoWrap
            };
            var layout = new CanvasTextLayout(session.Device, node.name, format, 0f, 0f);

            var offset = new Vector2(NODE_WIDTH + 15f, 0f);
            session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.White);
            offset.Y -= 25f;
            session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.Gray);
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center)
        {
            session.FillCircle(center, 5, Colors.LightGray);
            session.DrawCircle(center, 5, Colors.Black);
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
