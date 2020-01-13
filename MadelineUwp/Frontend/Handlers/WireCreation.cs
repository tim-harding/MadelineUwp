using Madeline.Backend;
using Madeline.Frontend.Structure;
using System;
using System.Numerics;

namespace Madeline.Frontend.Handlers
{
    internal class WireCreation : IMouseHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        public WireCreation(Viewport viewport, Mouse mouse)
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
            else if (isWireHover)
            {
                if (viewport.graph.nodes.TryGet(wire.node, out Node node))
                {
                    viewport.rewiring.src = wire;
                    viewport.rewiring.upstreamReference = node.inputs[wire.index];
                    viewport.rewiring.bidirectional = true;
                }
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
                // Should also be able to hover over a node to wire through it
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
            if (src.node < 0 || dst.node < 0)
            {
                rewiring.src = new Slot(-1, -1);
                return false;
            }

            if (rewiring.bidirectional)
            {
                bool dstIsOutput = dst.index < 0;
                int o = dstIsOutput ? dst.node : rewiring.upstreamReference;
                int i = dstIsOutput ? src.node : dst.node;
                int slot = Math.Max(src.index, dst.index);
                viewport.history.SubmitChange(new HistoricEvents.Connect(o, i, slot));
                rewiring.src = new Slot(-1, -1);
            }
            else
            {
                bool srcIsOutput = src.index < 0;
                int i = srcIsOutput ? dst.node : src.node;
                int o = srcIsOutput ? src.node : dst.node;
                int slot = Math.Max(src.index, dst.index);
                viewport.history.SubmitChange(new HistoricEvents.Connect(o, i, slot));
                rewiring.src = new Slot(-1, -1);
            }
            return true;
        }
    }
}
