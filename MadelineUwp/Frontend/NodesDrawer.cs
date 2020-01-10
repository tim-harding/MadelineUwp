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
        private struct CommandList
        {
            public CanvasCommandList list;
            public CanvasDrawingSession session;

            public CommandList(ICanvasResourceCreator device)
            {
                list = new CanvasCommandList(device);
                session = list.CreateDrawingSession();
            }
        }

        private struct Context
        {
            public CommandList wires;
            public CommandList nodes;
            public CommandList texts;

            public BaseGeo baseGeo;

            public Context(ICanvasResourceCreator device)
            {
                wires = new CommandList(device);
                nodes = new CommandList(device);
                texts = new CommandList(device);
                baseGeo = new BaseGeo(device);
            }
        }

        private struct BaseGeo
        {
            public CanvasGeometry disable;
            public CanvasGeometry clipper;
            public CanvasGeometry viewing;
            public CanvasGeometry slot;

            public BaseGeo(ICanvasResourceCreator device)
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

                slot = CanvasGeometry.CreateCircle(device, Vector2.Zero, Slot.DISPLAY_RADIUS);
            }
        }

        private Viewport viewport;
        private Mouse mouse;

        public NodesDrawer(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            var ctx = new Context(session.Device);
            session.Transform = viewport.Into();

            viewport.hover.Clear();

            Graph graph = viewport.graph;
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
                        DrawWire(ctx, iPos, oPos, new Slot(node.id, i));
                    }

                    DrawNodeIO(ctx, iPos, new Slot(node.id, i));
                }
                DrawNodeIO(ctx, node.value.OutputPos(), new Slot(node.id, -1));
                DrawNodeLabel(ctx, node.value);
            }

            session.DrawImage(ctx.wires.list);
            session.DrawImage(ctx.nodes.list);
            session.DrawImage(ctx.texts.list);
        }

        private void DrawNodeBody(TableEntry<Node> node, Context ctx, Plugin plugin)
        {
            var tx = Matrix3x2.CreateTranslation(node.value.pos);
            CanvasGeometry body = ctx.baseGeo.clipper.Transform(tx);
            CanvasGeometry disable = ctx.baseGeo.disable.Transform(tx);
            CanvasGeometry view = ctx.baseGeo.viewing.Transform(tx);

            bool hover = StoreNodeHover(body, node.id);

            bool active = node.id == viewport.selection.ActiveNode;
            bool enabled = node.value.enabled;
            bool selected = viewport.selection.active.nodes.Contains(node.id);
            if (selected)
            {
                ctx.nodes.session.DrawGeometry(body, Palette.Red5, 6f);
            }

            Color color = active || !enabled ? Palette.Black : Palette.Gray2;
            ctx.nodes.session.DrawGeometry(body, color, 2f);
            using (ctx.nodes.session.CreateLayer(1f, body))
            {
                bool candidate = viewport.selection.candidates.nodes.Contains(node.id);
                bool accent = hover || candidate;
                color = accent ? plugin.colors.hover : plugin.colors.body;
                color = enabled ? color : (accent ? Palette.Tone6 : Palette.Tone5);
                ctx.nodes.session.Clear(color);

                if (!enabled)
                {
                    ctx.nodes.session.FillGeometry(disable, Palette.Yellow5);
                }

                if (viewport.viewing == node.id)
                {
                    ctx.nodes.session.FillGeometry(view, Palette.Blue5);
                }

                color = enabled ? Palette.Tone7 : Palette.Black;
                ctx.nodes.session.DrawGeometry(disable, color);
                ctx.nodes.session.DrawGeometry(view, color);
            }
        }

        private bool StoreNodeHover(CanvasGeometry body, int node)
        {
            bool hoverAlreadyFound = viewport.hover.node > -1;
            if (hoverAlreadyFound) { return false; }

            Vector2 cursorLocal = viewport.From(mouse.current.pos);
            bool nodeHasHover = body.FillContainsPoint(cursorLocal);
            if (nodeHasHover)
            {
                viewport.hover.node = node;
            }
            return nodeHasHover;
        }

        private void DrawWire(Context ctx, Vector2 iPos, Vector2 oPos, Slot slot)
        {
            var wire = new Wire(iPos, oPos, WireKind.DoubleEnded);
            CanvasGeometry geo = wire.Geo(ctx.wires.session);
            bool hover = StoreWireHover(geo, slot);
            Color color = hover ? Palette.Indigo2 : Palette.Indigo4;
            ctx.wires.session.DrawGeometry(geo, color, 2f);
        }

        private bool StoreWireHover(CanvasGeometry geo, Slot wire)
        {
            bool hoverAlreadyFound = viewport.hover.wire.node > -1;
            if (hoverAlreadyFound) { return false; }

            Vector2 cursorLocal = viewport.From(mouse.current.pos);
            bool wireHasHover = geo.StrokeContainsPoint(cursorLocal, 16f);
            if (wireHasHover)
            {
                viewport.hover.wire = wire;
            }
            return wireHasHover;
        }

        private void DrawNodeLabel(Context ctx, Node node)
        {
            var format = new CanvasTextFormat()
            {
                FontSize = 18f,
                WordWrapping = CanvasWordWrapping.NoWrap,
            };
            var layout = new CanvasTextLayout(ctx.texts.session.Device, node.name, format, 0f, 0f);

            var offset = new Vector2(Node.Size.X + 15f, 0f);
            ctx.texts.session.DrawTextLayout(layout, node.pos + offset, Colors.White);
            offset.Y -= 25f;
            ctx.texts.session.DrawTextLayout(layout, node.pos + offset, Colors.Gray);
        }

        private void DrawNodeIO(Context ctx, Vector2 center, Slot slot)
        {
            bool hover = StoreIOHover(center, slot);
            Color color = hover ? Palette.White : Palette.Gray4;
            var tx = Matrix3x2.CreateTranslation(center);
            CanvasGeometry geo = ctx.baseGeo.slot.Transform(tx);
            ctx.nodes.session.FillGeometry(geo, color);
        }

        private bool StoreIOHover(Vector2 center, Slot slot)
        {
            Vector2 mouseLocal = viewport.From(mouse.current.pos);
            float distance = Vector2.DistanceSquared(center, mouseLocal);
            SlotProximity cmp = viewport.hover.slot;
            bool takeHover = distance < cmp.distance && !cmp.IsHover;
            if (!takeHover) { return false; }

            var proximity = new SlotProximity(distance, slot);
            viewport.hover.slot = proximity;
            return takeHover && proximity.IsHover;
        }
    }
}
