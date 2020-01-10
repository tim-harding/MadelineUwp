using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using System.Diagnostics;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class WireCreationDrawer : Drawer
    {
        private const string MISSING_PLUGIN_MESSAGE = "Nodes with missing plugins " +
            "have no input slots, so it should never " +
            "be possible to start a wire off them.";

        private Viewport viewport;
        private Mouse mouse;

        public WireCreationDrawer(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            Graph graph = viewport.graph;
            if (viewport.rewiring.src.node < 0) { return; }
            if (!graph.nodes.TryGet(viewport.rewiring.src.node, out Node srcNode)) { return; }

            if (!graph.plugins.TryGet(srcNode.plugin, out Plugin srcPlugin))
            {
                Debug.Assert(false, MISSING_PLUGIN_MESSAGE);
            }
            Vector2 srcPos = srcNode.SlotPos(viewport.rewiring.src.slot, srcPlugin.inputs);

            bool up = viewport.rewiring.src.slot > -1;
            if (graph.nodes.TryGet(viewport.rewiring.dst.node, out Node dstNode))
            {
                if (!graph.plugins.TryGet(dstNode.plugin, out Plugin dstPlugin))
                {
                    Debug.Assert(false, MISSING_PLUGIN_MESSAGE);
                }
                Vector2 dstPos = dstNode.SlotPos(viewport.rewiring.dst.slot, dstPlugin.inputs);
                if (!up)
                {
                    Swap(ref srcPos, ref dstPos);
                }
                var wire = new Wire(srcPos, dstPos);
                WireDrawer.DrawWire(session, wire, Palette.Indigo2, WireKind.DoubleEnded);
            }
            else
            {
                Vector2 dstPos = viewport.From(mouse.current.pos);
                WireKind kind = up ? WireKind.Up : WireKind.Down;
                if (!up)
                {
                    Swap(ref srcPos, ref dstPos);
                }
                var wire = new Wire(srcPos, dstPos);
                WireDrawer.DrawWire(session, wire, Palette.Indigo2, kind);
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
