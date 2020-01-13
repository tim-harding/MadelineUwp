using Madeline.Backend;
using Madeline.Frontend.Panes.NodeGraph.Structure;
using System.Numerics;

namespace Madeline.Frontend.Panes.NodeGraph.Drawing
{
    internal class WireCreationDrawer : IDrawer
    {
        private Viewport viewport;

        private Node src;
        private Node dst;

        public WireCreationDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        private bool Up => viewport.rewiring.src.index > -1;

        public void Draw()
        {
            RewiringInfo rewiring = viewport.rewiring;
            bool isRewiring = rewiring.src.node > -1;
            if (!isRewiring) { return; }

            if (Globals.graph.nodes.TryGet(rewiring.src.node, out Node srcNode))
            {
                src = srcNode;
                if (rewiring.bidirectional)
                {
                    Bidirectional();
                }
                else
                {
                    Directed();
                }
            }
        }

        private void Bidirectional()
        {
            bool hasDestination = viewport.rewiring.dst.node > -1;
            if (hasDestination)
            {
                BidirectionalSnapped();
            }
            else
            {
                BidirectionalDangling();
            }
        }

        private void Directed()
        {
            if (Globals.graph.nodes.TryGet(viewport.rewiring.dst.node, out Node dstNode))
            {
                dst = dstNode;
                DirectedSnapped();
            }
            else
            {
                DirectedDangling();
            }
        }

        private void BidirectionalSnapped()
        {
            // TODO: Should remove the previous connection
            RewiringInfo rewiring = viewport.rewiring;
            int dstNodeId = rewiring.dst.node;
            bool dstIsOutput = rewiring.dst.index < 0;
            int o = dstIsOutput ? dstNodeId : rewiring.upstream;
            int i = dstIsOutput ? rewiring.src.node : dstNodeId;

            Table<Node> nodes = Globals.graph.nodes;
            if (!nodes.TryGet(o, out Node oNode)) { return; }
            if (!nodes.TryGet(i, out Node iNode)) { return; }

            Vector2 iPos = iNode.InputPos(rewiring.src.index);
            Vector2 oPos = oNode.OutputPos();
            var wire = new Wire(iPos, oPos, Wire.Kind.DoubleEnded);
            Globals.session.DrawGeometry(wire.Geo(Globals.session), Palette.Indigo2);
        }

        private void BidirectionalDangling()
        {
            RewiringInfo rewiring = viewport.rewiring;
            Vector2 mousePos = viewport.From(Mouse.Relative);
            if (Globals.graph.nodes.TryGet(rewiring.upstream, out Node upstream))
            {
                Vector2 iPos = src.InputPos(rewiring.src.index);
                Vector2 oPos = upstream.OutputPos();
                var up = new Wire(iPos, mousePos, Wire.Kind.Up);
                var down = new Wire(mousePos, oPos, Wire.Kind.Down);
                Globals.session.DrawGeometry(up.Geo(Globals.session), Palette.Orange5);
                Globals.session.DrawGeometry(down.Geo(Globals.session), Palette.Orange5);
            }
        }

        private void DirectedSnapped()
        {
            RewiringInfo rewiring = viewport.rewiring;
            Vector2 srcPos = src.SlotPos(rewiring.src.index);
            Vector2 dstPos = dst.SlotPos(rewiring.dst.index);
            DirectedGeneral(srcPos, dstPos);
        }

        private void DirectedDangling()
        {
            Vector2 srcPos = src.SlotPos(viewport.rewiring.src.index);
            Vector2 dstPos = viewport.From(Mouse.Relative);
            DirectedGeneral(srcPos, dstPos);
        }

        private void DirectedGeneral(Vector2 srcPos, Vector2 dstPos)
        {
            Wire.Kind kind = Up ? Wire.Kind.Up : Wire.Kind.Down;
            if (!Up)
            {
                Utils.Swap(ref srcPos, ref dstPos);
            }
            var wire = new Wire(srcPos, dstPos, kind);
            Globals.session.DrawGeometry(wire.Geo(Globals.session), Palette.Indigo2);
        }
    }
}
