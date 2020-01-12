using Madeline.Backend;
using Madeline.Frontend.Structure;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.UI;

namespace Madeline.Frontend.Drawing.Nodes
{
    internal class Drawing : IDrawer
    {
        private Viewport viewport;
        private Mouse mouse;
        private Selection selection;

        public Drawing(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
            selection = new Selection(viewport, mouse);
        }

        public void Draw(CanvasDrawingSession session)
        {
            var ctx = new Context(session, viewport);
            session.Transform = viewport.Into();

            viewport.hover.Clear();
            viewport.selection.candidates.Clear();

            Graph graph = viewport.graph;
            foreach (TableEntry<Node> node in graph.nodes)
            {
                DrawNodeBody(node, ctx);
                for (int i = 0; i < node.value.plugin.inputs; i++)
                {
                    Vector2 iPos = node.value.InputPos(i, node.value.inputs.Length);
                    if (graph.nodes.TryGet(node.value.inputs[i], out Node upstream))
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

            DrawSelectBox(ctx);
        }

        private void DrawNodeBody(TableEntry<Node> node, Context ctx)
        {
            var tx = Matrix3x2.CreateTranslation(node.value.pos);
            BodyGeo body = ctx.geo.body.Transform(tx);

            bool hover = selection.StoreNodeHover(body, node.id);
            bool candidate = selection.StoreNodeSelectCandidacy(body.clipper, ctx.geo.selectBox, node.id);
            bool selected = viewport.selection.active.nodes.Contains(node.id);
            bool active = viewport.active == node.id;
            bool enabled = node.value.enabled;

            if (active || selected)
            {
                Color accentColor = active && !selected ? Palette.Red5 : Palette.Yellow5;
                ctx.nodes.session.DrawGeometry(body.clipper, accentColor, 6f);
            }

            Color bodyColor = active || selected || !enabled ? Palette.Black : Palette.Gray2;
            ctx.nodes.session.DrawGeometry(body.clipper, bodyColor, 2f);

            using (ctx.nodes.session.CreateLayer(1f, body.clipper))
            {
                bool accent = hover || candidate;
                Plugin.ColorScheme pluginColors = node.value.plugin.colors;
                bodyColor = accent ? pluginColors.hover : pluginColors.body;
                bodyColor = enabled ? bodyColor : (accent ? Palette.Tone6 : Palette.Tone5);
                ctx.nodes.session.Clear(bodyColor);

                bool disableHover = hover && viewport.hover.node.state == NodeHover.State.Disable;
                Color disableColor = disableHover ? Palette.Yellow3 : bodyColor;
                disableColor = enabled ? disableColor : Palette.Yellow5;
                ctx.nodes.session.FillGeometry(body.disable, disableColor);

                bool viewing = node.id == viewport.viewing;
                bool viewingHover = hover && viewport.hover.node.state == NodeHover.State.Viewing;
                Color viewingColor = viewingHover ? Palette.Blue3 : bodyColor;
                viewingColor = viewing ? Palette.Blue5 : viewingColor;
                ctx.nodes.session.FillGeometry(body.viewing, viewingColor);

                viewingColor = enabled ? Palette.Tone7 : Palette.Black;
                ctx.nodes.session.DrawGeometry(body.disable, viewingColor);
                ctx.nodes.session.DrawGeometry(body.viewing, viewingColor);
            }
        }

        private void DrawWire(Context ctx, Vector2 iPos, Vector2 oPos, Slot slot)
        {
            var wire = new Wire(iPos, oPos, Wire.Kind.DoubleEnded);
            CanvasGeometry geo = wire.Geo(ctx.wires.session);
            bool hover = selection.StoreWireHover(geo, slot);
            bool candidate = selection.StoreWireSelectCandidacy(geo, ctx.geo.selectBox, slot);
            bool selected = viewport.selection.active.wires.Contains(slot);
            Color color = hover || candidate ? Palette.Indigo2 : Palette.Indigo4;
            color = selected ? Palette.Yellow3 : color;
            ctx.wires.session.DrawGeometry(geo, color, 2f);
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
            bool hover = selection.StoreIOHover(center, slot);
            Color color = hover ? Palette.White : Palette.Gray4;
            var tx = Matrix3x2.CreateTranslation(center);
            CanvasGeometry geo = ctx.geo.slot.Transform(tx);
            ctx.nodes.session.FillGeometry(geo, color);
        }

        private void DrawSelectBox(Context ctx)
        {
            var color = Color.FromArgb(64, 255, 255, 255);
            CanvasGeometry bbox = ctx.geo.selectBox;
            ctx.session.DrawGeometry(bbox, color);
            ctx.session.FillGeometry(bbox, color);
        }
    }
}
