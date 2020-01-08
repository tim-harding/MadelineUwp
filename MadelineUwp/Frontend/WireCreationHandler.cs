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
            Slot slot = viewport.hover.slot;
            bool valid = slot.node > -1;
            if (valid)
            {
                viewport.wireSrc = slot;
            }
            return valid;
        }

        private bool AdvancePull()
        {
            if (viewport.wireSrc.node < 0)
            {
                return false;
            }

            Graph graph = viewport.graph;
            int srcNode = viewport.wireSrc.node;
            Vector2 srcPos = graph.nodes.Get(srcNode).pos;

            float nearest = float.MaxValue;
            viewport.wireDst = new Slot(-1, -1);
            foreach (TableRow<Node> node in graph.nodes)
            {
                if (node.id == srcNode)
                {
                    continue;
                }

                bool srcIsOutput = viewport.wireSrc.slot < 0;
                if (srcIsOutput)
                {
                    Plugin plugin = graph.plugins.Get(node.value.plugin);
                    for (int i = 0; i < plugin.inputs; i++)
                    {
                        Vector2 iPos = node.value.InputPos(i, plugin.inputs);
                        SetNearest(srcPos, iPos, new Slot(node.id, i), ref nearest);
                    }
                }
                else
                {
                    Vector2 oPos = node.value.OutputPos();
                    SetNearest(srcPos, oPos, new Slot(node.id, -1), ref nearest);
                }
            }

            const float SNAP_RADIUS = 1024f;
            viewport.wireDst = nearest < SNAP_RADIUS ? viewport.wireDst : new Slot(-1, -1);
            return true;
        }

        private void SetNearest(Vector2 srcPos, Vector2 dstPos, Slot slot, ref float nearest)
        {
            float distance = Vector2.DistanceSquared(srcPos, dstPos);
            if (distance < nearest)
            {
                nearest = distance;
                viewport.wireDst = slot;
            }
        }

        private bool CommitPull()
        {
            if (viewport.wireSrc.node < 0)
            {
                return false;
            }
            Slot src = viewport.wireSrc;
            Slot dst = viewport.wireDst;
            bool srcIsOutput = src.slot < 0;
            int i = srcIsOutput ? dst.node : src.node;
            int o = srcIsOutput ? src.node : dst.node;
            int slot = Math.Max(src.slot, dst.slot);
            viewport.graph.Connect(o, i, slot);
            viewport.wireSrc = new Slot(-1, -1);
            return true;
        }
    }
}
