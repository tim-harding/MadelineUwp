using Madeline.Backend;
using System.Numerics;
using Windows.Foundation;

namespace Madeline
{
    internal class Hover
    {
        private Mouse mouse;
        private Viewport viewport;

        public Hover(Mouse mouse, Viewport viewport)
        {
            this.mouse = mouse;
            this.viewport = viewport;
        }

        public void HandleMouse()
        {
            Graph graph = viewport.graph;
            graph.hoverNode = -1;
            graph.hoverSlot = new Slot(-1, -1);
            graph.hoverWire = new Slot(-1, -1);

            var pos = viewport.From(mouse.current.pos).ToPoint();
            foreach ((int id, Node value) node in viewport.graph.nodes)
            {
                TrySetNode(node);
                UpdateActiveSlot(node);
            }
        }

        private void UpdateActiveSlot((int id, Node value) node)
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

        private void TrySetNode((int id, Node value) node)
        {
            var rect = new Rect(node.value.pos.ToPoint(), Node.Size.ToSize());
            var pos = viewport.From(mouse.current.pos).ToPoint();
            if (rect.Contains(pos))
            {
                viewport.graph.hoverNode = node.id;
            }
        }

        private void TrySetSlot(int nodeId, Vector2 pos, int slot)
        {
            const float SLOT_SELECT_RANGE = 256f;
            if (Vector2.DistanceSquared(viewport.Into(pos), mouse.current.pos) < SLOT_SELECT_RANGE)
            {
                viewport.graph.hoverSlot = new Slot(nodeId, slot);
            }
        }

        private void UpdateActiveWire(Vector2 iPos, int oNodeId, int iNodeId, int slot)
        {
            Graph graph = viewport.graph;
            const float WIRE_SELECT_RANGE = 256f;
            if (!graph.nodes.TryGet(oNodeId, out Node oNode))
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
                graph.hoverWire = new Slot(iNodeId, slot);
            }
        }
    }
}
