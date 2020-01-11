using Madeline.Backend;
using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class WireCreationHandler : MouseHandler
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
            bool valid = proximity.slot.node > -1 && proximity.IsHover;
            if (valid)
            {
                viewport.rewiring.src = proximity.slot;
            }
            return valid;
        }

        private bool AdvancePull()
        {
            Graph graph = viewport.graph;
            int srcId = viewport.rewiring.src.node;
            if (!graph.nodes.TryGet(srcId, out Node srcNode)) { return false; }

            RewiringInfo rewiring = viewport.rewiring;
            Vector2 cursor = viewport.From(mouse.current.pos);
            float nearest = float.MaxValue;
            rewiring.dst = new Slot(-1, -1);
            foreach (TableEntry<Node> node in graph.nodes)
            {
                if (node.id == srcId) { continue; }

                bool srcIsOutput = rewiring.src.slot < 0;
                if (srcIsOutput)
                {
                    int inputs = node.value.inputs.Length;
                    for (int i = 0; i < inputs; i++)
                    {
                        Vector2 iPos = node.value.InputPos(i, inputs);
                        SetNearest(cursor, iPos, new Slot(node.id, i), ref nearest);
                    }
                }
                else
                {
                    Vector2 oPos = node.value.OutputPos();
                    SetNearest(cursor, oPos, new Slot(node.id, -1), ref nearest);
                }
            }

            const float SNAP_RADIUS = 1024f;
            rewiring.dst = nearest < SNAP_RADIUS ? rewiring.dst : Slot.Empty;
            return true;
        }

        private void SetNearest(Vector2 srcPos, Vector2 dstPos, Slot slot, ref float nearest)
        {
            float distance = Vector2.DistanceSquared(srcPos, dstPos);
            if (distance < nearest)
            {
                nearest = distance;
                viewport.rewiring.dst = slot;
            }
        }

        private bool CommitPull()
        {
            RewiringInfo rewiring = viewport.rewiring;
            if (rewiring.src.node < 0 || rewiring.dst.node < 0)
            {
                rewiring.src = new Slot(-1, -1);
                return false;
            }

            Slot src = rewiring.src;
            Slot dst = rewiring.dst;
            bool srcIsOutput = src.slot < 0;
            int i = srcIsOutput ? dst.node : src.node;
            int o = srcIsOutput ? src.node : dst.node;
            int slot = Math.Max(src.slot, dst.slot);
            viewport.history.SubmitChange(new HistoricEvents.Connect(o, i, slot));
            rewiring.src = new Slot(-1, -1);
            return true;
        }
    }
}
