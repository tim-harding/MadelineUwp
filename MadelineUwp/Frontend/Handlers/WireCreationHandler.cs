using Madeline.Backend;
using Madeline.Frontend.Structure;
using System;

namespace Madeline.Frontend.Handlers
{
    internal class WireCreationHandler : IMouseHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        public WireCreationHandler(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public bool HandleMouse()
        {
            switch (mouse.Left)
            {
                case MouseButton.Down:
                    return BeginPull();

                case MouseButton.Dragging:
                    return AdvancePull();

                case MouseButton.Up:
                    return CommitPull();
            }
            return false;
        }

        private bool BeginPull()
        {
            SlotProximity proximity = viewport.hover.slot;
            bool isSlotHover = proximity.slot.node > -1 && proximity.IsHover;

            Slot wire = viewport.hover.wire;
            bool isWireHover = wire.node > -1;

            if (isSlotHover)
            {
                viewport.rewiring.src = proximity.slot;
                viewport.rewiring.bidirectional = false;
            }
            else if (isWireHover && viewport.graph.nodes.TryGet(wire.node, out Node node))
            {
                viewport.rewiring.src = wire;
                viewport.rewiring.upstream = node.inputs[wire.index];
                viewport.rewiring.bidirectional = true;
            }
            viewport.rewiring.dst = Slot.Empty;

            bool valid = isSlotHover || isWireHover;
            return valid;
        }

        private bool AdvancePull()
        {
            Graph graph = viewport.graph;
            Slot src = viewport.rewiring.src;
            if (!graph.nodes.TryGet(src.node, out Node srcNode)) { return false; }

            RewiringInfo rewiring = viewport.rewiring;

            SlotProximity nearest = viewport.hover.slot;
            bool srcIsOutput = rewiring.src.index < 0;

            const float SNAP_RADIUS = 1024f;
            bool isNear = nearest.distance < SNAP_RADIUS;
            if (nearest.slot.node == src.node)
            {
                rewiring.dst = Slot.Empty;
            }
            else if (rewiring.bidirectional)
            {
                // TODO: Should also be able to hover over a node to wire through it
                rewiring.dst = isNear ? nearest.slot : Slot.Empty;
            }
            else if (srcIsOutput)
            {
                bool isInput = nearest.slot.index > -1;
                rewiring.dst = isInput && isNear ? nearest.slot : Slot.Empty;
            }
            else
            {
                bool isOutput = nearest.slot.index < 0;
                rewiring.dst = isOutput && isNear ? nearest.slot : Slot.Empty;
            }

            return true;
        }

        private bool CommitPull()
        {
            RewiringInfo rewiring = viewport.rewiring;
            Slot src = rewiring.src;
            Slot dst = rewiring.dst;
            bool complete = src.node > -1 && dst.node > -1;
            if (complete)
            {
                (int i, int o) = RewiringIO();
                int slot = Math.Max(src.index, dst.index);
                viewport.history.SubmitChange(new HistoricEvents.Connect(o, i, slot));
            }
            rewiring.src = new Slot(-1, -1);
            return complete;
        }

        private (int i, int o) RewiringIO()
        {
            RewiringInfo rewiring = viewport.rewiring;
            Slot src = rewiring.src;
            Slot dst = rewiring.dst;
            int i, o;
            if (rewiring.bidirectional)
            {
                bool dstIsOutput = dst.index < 0;
                i = dstIsOutput ? src.node : dst.node;
                o = dstIsOutput ? dst.node : rewiring.upstream;
            }
            else
            {
                bool srcIsOutput = src.index < 0;
                i = srcIsOutput ? dst.node : src.node;
                o = srcIsOutput ? src.node : dst.node;
            }
            return (i, o);
        }
    }
}
