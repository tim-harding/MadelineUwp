using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Madeline.Frontend.Drawers
{
    internal class WireCreation : Drawer
    {
        private Viewport viewport;
        private Mouse mouse;

        public WireCreation(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            if (viewport.rewiring.src.node < 0) { return; }
            if (!graph.nodes.TryGet(viewport.rewiring.src.node, out Node srcNode)) { return; }

            Vector2 srcPos = srcNode.SlotPos(viewport.rewiring.src.slot, srcNode.inputs.Length);

            bool up = viewport.rewiring.src.slot > -1;
            if (graph.nodes.TryGet(viewport.rewiring.dst.node, out Node dstNode))
            {
                Vector2 dstPos = dstNode.SlotPos(viewport.rewiring.dst.slot, dstNode.inputs.Length);
                if (!up)
                {
                    Swap(ref srcPos, ref dstPos);
                }
                var wire = new Wire(srcPos, dstPos, WireKind.DoubleEnded);
                session.DrawGeometry(wire.Geo(session), Palette.Indigo2);
            }
            else
            {
                Vector2 dstPos = viewport.From(mouse.current.pos);
                WireKind kind = up ? WireKind.Up : WireKind.Down;
                if (!up)
                {
                    Swap(ref srcPos, ref dstPos);
                }
                var wire = new Wire(srcPos, dstPos, kind);
                session.DrawGeometry(wire.Geo(session), Palette.Indigo2);
            }
        }

        private void Swap(ref Vector2 lhs, ref Vector2 rhs)
        {
            Vector2 tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
