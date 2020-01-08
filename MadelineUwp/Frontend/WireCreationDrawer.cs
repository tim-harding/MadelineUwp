using Madeline.Backend;
using Microsoft.Graphics.Canvas;
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
            if (viewport.wireSrc.node < 0)
            {
                return;
            }
            if (!graph.nodes.TryGet(viewport.wireSrc.node, out Node srcNode))
            {
                return;
            }

            if (!graph.plugins.TryGet(srcNode.plugin, out Plugin srcPlugin))
            {
                System.Diagnostics.Debug.Assert(false, MISSING_PLUGIN_MESSAGE);
            }
            Vector2 srcPos = viewport.Into(srcNode.SlotPos(viewport.wireSrc.slot, srcPlugin.inputs));
            if (graph.nodes.TryGet(viewport.wireDst.node, out Node dstNode))
            {
                if (!graph.plugins.TryGet(dstNode.plugin, out Plugin dstPlugin))
                {
                    System.Diagnostics.Debug.Assert(false, MISSING_PLUGIN_MESSAGE);
                }
                Vector2 dstPos = viewport.Into(dstNode.SlotPos(viewport.wireDst.slot, dstPlugin.inputs));
                WireDrawer.DrawWire(session, srcPos, dstPos, Palette.Indigo2, viewport.zoom, true);
            }
            else
            {
                Vector2 dstPos = mouse.current.pos;
                WireDrawer.DrawWire(session, srcPos, dstPos, Palette.Indigo2, viewport.zoom, false);
            }
        }
    }
}
