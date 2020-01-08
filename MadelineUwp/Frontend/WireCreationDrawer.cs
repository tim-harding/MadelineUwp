using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class WireCreationDrawer : Drawer
    {
        private Viewport viewport;
        private Mouse mouse;

        public WireCreationDrawer(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            if (viewport.wireSrc.node < 0)
            {
                return;
            }
            if (!viewport.graph.nodes.TryGet(viewport.wireSrc.node, out Node srcNode))
            {
                return;
            }

            Plugin srcPlugin = viewport.graph.plugins.Get(srcNode.plugin);
            Vector2 srcPos = viewport.Into(srcNode.SlotPos(viewport.wireSrc.slot, srcPlugin.inputs));
            if (viewport.graph.nodes.TryGet(viewport.wireDst.node, out Node dstNode))
            {
                Plugin dstPlugin = viewport.graph.plugins.Get(dstNode.plugin);
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
