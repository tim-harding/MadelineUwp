﻿using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.UI;

namespace Madeline.Frontend.Drawers
{
    internal class Nodes : Drawer
    {
        private Viewport viewport;
        private Mouse mouse;

        public Nodes(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            var ctx = new Context(session.Device, viewport);
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

            var color = Color.FromArgb(64, 255, 255, 255);
            CanvasGeometry bbox = ctx.geo.selectBox;
            session.DrawGeometry(bbox, color);
            session.FillGeometry(bbox, color);
        }

        private void DrawNodeBody(TableEntry<Node> node, Context ctx)
        {
            var tx = Matrix3x2.CreateTranslation(node.value.pos);
            BodyGeo body = ctx.geo.body.Transform(tx);

            bool hover = StoreNodeHover(body, node.id);
            bool candidate = StoreNodeSelectCandidacy(body.clipper, ctx.geo.selectBox, node.id);
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

        private bool StoreNodeHover(BodyGeo body, int node)
        {
            bool hoverAlreadyFound = viewport.hover.node.id > -1;
            if (hoverAlreadyFound) { return false; }

            Vector2 cursorLocal = viewport.From(mouse.current.pos);
            bool nodeHasHover = body.clipper.FillContainsPoint(cursorLocal);
            if (!nodeHasHover) { return false; }

            bool disable = body.disable.FillContainsPoint(cursorLocal);
            bool viewing = body.viewing.FillContainsPoint(cursorLocal);
            NodeHover.State state = disable ? NodeHover.State.Disable : NodeHover.State.Body;
            state = viewing ? NodeHover.State.Viewing : state;
            viewport.hover.node = new NodeHover(node, state);
            return true;
        }

        private bool StoreNodeSelectCandidacy(CanvasGeometry clipper, CanvasGeometry bbox, int node)
        {
            switch (clipper.CompareWith(bbox))
            {
                case CanvasGeometryRelation.Disjoint:
                    return false;
                default:
                    viewport.selection.candidates.nodes.Add(node);
                    return true;
            }
        }

        private void DrawWire(Context ctx, Vector2 iPos, Vector2 oPos, Slot slot)
        {
            var wire = new Wire(iPos, oPos, WireKind.DoubleEnded);
            CanvasGeometry geo = wire.Geo(ctx.wires.session);
            bool hover = StoreWireHover(geo, slot);
            bool candidate = StoreWireSelectCandidacy(geo, ctx.geo.selectBox, slot);
            bool selected = viewport.selection.active.wires.Contains(slot);
            Color color = hover || candidate ? Palette.Indigo2 : Palette.Indigo4;
            color = selected ? Palette.Yellow3 : color;
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

        private bool StoreWireSelectCandidacy(CanvasGeometry geo, CanvasGeometry bbox, Slot wire)
        {
            switch (geo.CompareWith(bbox))
            {
                case CanvasGeometryRelation.Disjoint:
                    return false;
                default:
                    viewport.selection.candidates.wires.Add(wire);
                    return true;
            }
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
            CanvasGeometry geo = ctx.geo.slot.Transform(tx);
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
