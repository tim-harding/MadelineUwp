using Madeline.Backend;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend
{
    internal class Hover : MouseHandler
    {
        private Mouse mouse;
        private Viewport viewport;

        public Hover(Mouse mouse, Viewport viewport)
        {
            this.mouse = mouse;
            this.viewport = viewport;
        }

        public bool HandleMouse()
        {
            viewport.hover.Clear();
            var pos = viewport.From(mouse.current.pos).ToPoint();
            foreach (TableRow<Node> node in viewport.graph.nodes)
            {
                TrySetNode(node);
                UpdateActiveSlot(node);
            }
            return false;
        }

        private void UpdateActiveSlot(TableRow<Node> node)
        {
            Graph graph = viewport.graph;
            Plugin plugin = graph.plugins.Get(node.value.plugin);

            ListSlice<int> inputs = graph.inputs.Get(node.id);
            for (int i = 0; i < plugin.inputs; i++)
            {
                Vector2 iPos = node.value.InputPos(i, plugin.inputs);
                TrySetSlot(node.id, iPos, i);
                UpdateActiveWire(iPos, inputs.Consume(), node.id, i);
            }

            TrySetSlot(node.id, node.value.OutputPos(), -1);
        }

        private void TrySetNode(TableRow<Node> node)
        {
            var rect = new Rect(node.value.pos.ToPoint(), Node.Size.ToSize());
            var pos = viewport.From(mouse.current.pos).ToPoint();
            if (rect.Contains(pos))
            {
                viewport.hover.node = node.id;
            }
        }

        private void TrySetSlot(int nodeId, Vector2 pos, int slot)
        {
            const float SLOT_SELECT_RANGE = 256f;
            if (Vector2.DistanceSquared(viewport.Into(pos), mouse.current.pos) < SLOT_SELECT_RANGE)
            {
                viewport.hover.slot = new Slot(nodeId, slot);
            }
        }

        private void UpdateActiveWire(Vector2 iPos, int oNodeId, int iNodeId, int slot)
        {
            const float WIRE_SELECT_RANGE = 256f;
            if (!viewport.graph.nodes.TryGet(oNodeId, out Node oNode))
            {
                return;
            }
            Vector2 oPos = oNode.OutputPos();

            Vector2 start = viewport.Into(iPos);
            Vector2 end = viewport.Into(oPos);
            Vector2 dir = end - start;
            float len = dir.Length();
            dir /= len;

            float t = Vector2.Dot(mouse.current.pos - start, dir);
            Vector2 proj = t * dir + start;
            float dist = Vector2.DistanceSquared(mouse.current.pos, proj);
            bool inRange = dist < WIRE_SELECT_RANGE;
            bool onSegment = t < len && t > 0;
            if (inRange && onSegment)
            {
                viewport.hover.wire = new Slot(iNodeId, slot);
            }
        }
    }
}
