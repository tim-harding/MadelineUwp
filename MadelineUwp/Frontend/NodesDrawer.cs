using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline.Frontend
{
    internal class NodesDrawer : Drawer
    {
        private struct Context
        {
            public CanvasDrawingSession session;

            public CanvasCommandList wires;
            public CanvasCommandList bodies;
            public CanvasCommandList texts;

            public NodeGeo nodeGeo;

            public Context(CanvasDrawingSession session)
            {
                this.session = session;
                CanvasDevice device = session.Device;
                wires = new CanvasCommandList(device);
                bodies = new CanvasCommandList(device);
                texts = new CanvasCommandList(device);
                nodeGeo = new NodeGeo(device);
            }
        }

        private struct NodeGeo
        {
            public CanvasGeometry disable;
            public CanvasGeometry clipper;
            public CanvasGeometry viewing;

            public NodeGeo(ICanvasResourceCreator device)
            {
                var rect = new Rect(Vector2.Zero.ToPoint(), Node.Size.ToSize());
                const float ROUNDING = 5f;
                clipper = CanvasGeometry.CreateRoundedRectangle(device, rect, ROUNDING, ROUNDING);

                var size = new Vector2(20f, 60f);
                rect = new Rect((-size).ToPoint(), size.ToPoint());
                var verticalCenter = Matrix3x2.CreateTranslation(0f, Node.Size.Y / 2f);
                var rotate = Matrix3x2.CreateRotation(0.2f);
                disable = CanvasGeometry.CreateRectangle(device, rect);
                disable = disable.Transform(rotate * verticalCenter);
                var farSideTx = Matrix3x2.CreateTranslation(Node.Size.X, 0f);
                viewing = disable.Transform(farSideTx);
            }
        }

        private Viewport viewport;

        public NodesDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession session)
        {
            var ctx = new Context(session);
            session.Transform = viewport.Into();

            Graph graph = viewport.graph;
            Slot slot = viewport.hover.slot.Hover();
            foreach (TableEntry<Node> node in graph.nodes)
            {
                if (!graph.plugins.TryGet(node.value.plugin, out Plugin plugin))
                {
                    Debug.Assert(false, "Incomplete handling for missing plugins.");
                }
                DrawNodeBody(node, ctx, plugin);
                ListSlice<int> inputs = graph.inputs.GetAtRow(node.row);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 iPos = node.value.InputPos(i, plugin.inputs);
                    if (graph.nodes.TryGet(inputs.Consume(), out Node upstream))
                    {
                        Vector2 oPos = upstream.OutputPos();
                        DrawWire(session, iPos, oPos, viewport.hover.wire.Hover().Equals(new Slot(node.id, i)));
                    }

                    DrawNodeIO(session, iPos, slot.Equals(new Slot(node.id, i)));
                }
                DrawNodeIO(session, node.value.OutputPos(), slot.Equals(new Slot(node.id, -1)));
                DrawNodeLabel(session, node.value);
            }

            session.DrawImage(ctx.wires);
            session.DrawImage(ctx.bodies);
            session.DrawImage(ctx.texts);
        }

        private void DrawNodeBody(TableEntry<Node> node, Context ctx, Plugin plugin)
        {
            var tx = Matrix3x2.CreateTranslation(node.value.pos);
            CanvasGeometry body = ctx.nodeGeo.clipper.Transform(tx);
            CanvasGeometry disable = ctx.nodeGeo.disable.Transform(tx);
            CanvasGeometry view = ctx.nodeGeo.viewing.Transform(tx);

            CanvasDrawingSession session = ctx.session;
            bool active = node.id == viewport.selection.ActiveNode;
            bool enabled = node.value.enabled;
            bool selected = viewport.selection.active.nodes.Contains(node.id);
            if (selected)
            {
                session.DrawGeometry(body, Palette.Red5, 6f);
            }

            Color color = active || !enabled ? Palette.Black : Palette.Gray2;
            session.DrawGeometry(body, color, 2f);
            using (session.CreateLayer(1f, body))
            {
                bool hover = viewport.hover.node == node.id;
                bool candidate = viewport.selection.candidates.nodes.Contains(node.id);
                bool accent = hover || candidate;
                color = accent ? plugin.colors.hover : plugin.colors.body;
                color = enabled ? color : (accent ? Palette.Tone6 : Palette.Tone5);
                session.Clear(color);

                if (!enabled)
                {
                    session.FillGeometry(disable, Palette.Yellow5);
                }

                if (viewport.viewing == node.id)
                {
                    session.FillGeometry(view, Palette.Blue5);
                }

                color = enabled ? Palette.Tone7 : Palette.Black;
                session.DrawGeometry(disable, color);
                session.DrawGeometry(view, color);
            }
        }

        private void DrawWire(CanvasDrawingSession session, Vector2 iPos, Vector2 oPos, bool hover)
        {
            Color color = hover ? Palette.Indigo2 : Palette.Indigo4;
            var wire = new Wire(iPos, oPos);
            WireDrawer.DrawWire(session, wire, color, WireKind.DoubleEnded);
        }

        private void DrawNodeLabel(CanvasDrawingSession session, Node node)
        {
            var format = new CanvasTextFormat()
            {
                FontSize = 18f,
                WordWrapping = CanvasWordWrapping.NoWrap,
            };
            var layout = new CanvasTextLayout(session.Device, node.name, format, 0f, 0f);

            var offset = new Vector2(Node.Size.X + 15f, 0f);
            session.DrawTextLayout(layout, node.pos + offset, Colors.White);
            offset.Y -= 25f;
            session.DrawTextLayout(layout, node.pos + offset, Colors.Gray);
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center, bool hover)
        {
            Color color = hover ? Palette.White : Palette.Gray4;
            session.FillCircle(center, Slot.DISPLAY_RADIUS, color);
        }
    }
}
