using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Madeline.Frontend.Drawing
{
    internal class WireCreation : IDrawer
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

            Vector2 srcPos = srcNode.SlotPos(viewport.rewiring.src.index, srcNode.inputs.Length);

            bool up = viewport.rewiring.src.index > -1;
            if (viewport.rewiring.bidirectional)
            {
                if (viewport.graph.nodes.TryGet(viewport.rewiring.upstreamReference, out Node prevNode))
                {
                    Vector2 mousePos = viewport.From(mouse.current.pos);
                    if (viewport.rewiring.dst.node < 0)
                    {
                        var first = new Wire(srcNode.InputPos(viewport.rewiring.src.index), mousePos, Wire.Kind.Up);
                        var second = new Wire(mousePos, prevNode.OutputPos(), Wire.Kind.Down);
                        session.DrawGeometry(first.Geo(session), Palette.Orange5);
                        session.DrawGeometry(second.Geo(session), Palette.Orange5);
                    }
                    else
                    {
                        int dstNodeId = viewport.rewiring.dst.node;
                        bool dstIsOutput = viewport.rewiring.dst.index < 0;
                        int o = dstIsOutput ? dstNodeId : viewport.rewiring.upstreamReference;
                        int i = dstIsOutput ? viewport.rewiring.src.node : dstNodeId;

                        if (!graph.nodes.TryGet(o, out Node oNode)) { return; }
                        if (!graph.nodes.TryGet(i, out Node iNode)) { return; }

                        var wire = new Wire(iNode.InputPos(viewport.rewiring.src.index), oNode.OutputPos(), Wire.Kind.DoubleEnded);
                        session.DrawGeometry(wire.Geo(session), Palette.Indigo2);
                    }
                }
            }
            else
            {
                if (graph.nodes.TryGet(viewport.rewiring.dst.node, out Node dstNode))
                {
                    Vector2 dstPos = dstNode.SlotPos(viewport.rewiring.dst.index, dstNode.inputs.Length);
                    if (!up)
                    {
                        Swap(ref srcPos, ref dstPos);
                    }
                    var wire = new Wire(srcPos, dstPos, Wire.Kind.DoubleEnded);
                    session.DrawGeometry(wire.Geo(session), Palette.Indigo2);
                }
                else
                {
                    Vector2 dstPos = viewport.From(mouse.current.pos);
                    Wire.Kind kind = up ? Wire.Kind.Up : Wire.Kind.Down;
                    if (!up)
                    {
                        Swap(ref srcPos, ref dstPos);
                    }
                    var wire = new Wire(srcPos, dstPos, kind);
                    session.DrawGeometry(wire.Geo(session), Palette.Indigo2);
                }
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
