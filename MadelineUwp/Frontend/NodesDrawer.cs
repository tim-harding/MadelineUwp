using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline.Frontend
{
    internal class NodesDrawer : Drawer
    {
        private const float IO_RADIUS = 4.5f;

        private Viewport viewport;

        public NodesDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            Slot slot = viewport.hover.slot;
            foreach (TableRow<Node> node in graph.nodes)
            {
                Plugin plugin = graph.plugins.Get(node.value.plugin);
                DrawNodeBody(node, session, plugin);
                ListSlice<int> inputs = graph.inputs.Get(node.id);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 iPos = node.value.InputPos(i, plugin.inputs);
                    if (graph.nodes.TryGet(inputs.Consume(), out Node upstream))
                    {
                        Vector2 oPos = upstream.OutputPos();
                        DrawWire(session, iPos, oPos, viewport.hover.wire.Match(node.id, i));
                    }

                    DrawNodeIO(session, iPos, slot.Match(node.id, i));
                }
                DrawNodeIO(session, node.value.OutputPos(), slot.Match(node.id, -1));
                DrawNodeLabel(session, node.value);
            }
        }

        private void DrawNodeBody(TableRow<Node> node, CanvasDrawingSession session, Plugin plugin)
        {
            Vector2 upperLeft = viewport.Into(node.value.pos);
            Matrix3x2 tx = Matrix3x2.CreateScale(viewport.zoom) * Matrix3x2.CreateTranslation(upperLeft);

            var rect = new Rect(Vector2.Zero.ToPoint(), Node.Size.ToSize());
            const float ROUNDING = 5f;
            var body = CanvasGeometry.CreateRoundedRectangle(session.Device, rect, ROUNDING, ROUNDING);
            var bgFillShapeRect = new Rect((-Vector2.One * 4f).ToPoint(), (Node.Size + Vector2.One * 4f).ToPoint());
            var bgFillShape = CanvasGeometry.CreateRectangle(session.Device, bgFillShapeRect);

            var size = new Vector2(20f, 60f);
            rect = new Rect((-size).ToPoint(), size.ToPoint());
            var verticalCenter = Matrix3x2.CreateTranslation(0f, Node.Size.Y / 2f);
            var rotate = Matrix3x2.CreateRotation(0.2f);
            var disable = CanvasGeometry.CreateRectangle(session.Device, rect);
            disable = disable.Transform(rotate * verticalCenter);
            var farSideTx = Matrix3x2.CreateTranslation(Node.Size.X, 0f);
            CanvasGeometry view = disable.Transform(farSideTx);

            body = body.Transform(tx);
            disable = disable.Transform(tx);
            view = view.Transform(tx);
            bgFillShape = bgFillShape.Transform(tx);

            bool active = node.id == viewport.active.node;
            bool enabled = node.value.enabled;
            bool selected = viewport.selection.nodes.Contains(node.id);
            if (selected)
            {
                session.DrawGeometry(body, Palette.Red5, 6f);
            }

            Color color = active || !enabled ? Palette.Black : Palette.Gray2;
            session.DrawGeometry(body, color, 2f);
            using (session.CreateLayer(1f, body))
            {
                bool hover = viewport.hover.node == node.id;
                bool candidate = viewport.selectionCandidates.nodes.Contains(node.id);
                bool accent = hover || candidate;
                color = accent ? plugin.colors.hover : plugin.colors.body;
                color = enabled ? color : (accent ? Palette.Tone6 : Palette.Tone5);
                session.FillGeometry(bgFillShape, color);

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
            Vector2 slotEnd = Vector2.UnitY * IO_RADIUS;
            oPos += slotEnd;
            iPos -= slotEnd;
            iPos = viewport.Into(iPos);
            oPos = viewport.Into(oPos);

            Color color = hover ? Palette.Indigo2 : Palette.Indigo4;
            WireDrawer.DrawWire(session, iPos, oPos, color, viewport.zoom, false);
        }

        private void DrawNodeLabel(CanvasDrawingSession session, Node node)
        {
            var format = new CanvasTextFormat()
            {
                FontSize = 18f * viewport.zoom,
                WordWrapping = CanvasWordWrapping.NoWrap
            };
            var layout = new CanvasTextLayout(session.Device, node.name, format, 0f, 0f);

            var offset = new Vector2(Node.Size.X + 15f, 0f);
            session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.White);
            offset.Y -= 25f;
            session.DrawTextLayout(layout, viewport.Into(node.pos + offset), Colors.Gray);
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center, bool hover)
        {
            Color color = hover ? Palette.White : Palette.Gray4;
            session.FillCircle(viewport.Into(center), IO_RADIUS * viewport.zoom, color);
        }
    }
}
