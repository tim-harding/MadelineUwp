using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;

namespace Madeline.Frontend.Drawing.Nodes
{
    internal class Selection
    {
        private Viewport viewport;
        private Mouse mouse;

        public Selection(Viewport viewport, Mouse mouse)
        {
            this.mouse = mouse;
            this.viewport = viewport;
        }

        public bool StoreWireHover(CanvasGeometry geo, Slot wire)
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

        public bool StoreWireSelectCandidacy(CanvasGeometry geo, CanvasGeometry bbox, Slot wire)
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

        public bool StoreIOHover(Vector2 center, Slot slot)
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

        public bool StoreNodeHover(BodyGeo body, int node)
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

        public bool StoreNodeSelectCandidacy(CanvasGeometry clipper, CanvasGeometry bbox, int node)
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
    }
}
