using Madeline.Backend;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend
{
    internal class Hover : MouseHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        public Hover(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public bool HandleMouse()
        {
            viewport.hover.Clear();
            var pos = viewport.From(mouse.current.pos).ToPoint();
            foreach (TableEntry<Node> node in viewport.graph.nodes)
            {
                TrySetNode(node);
                UpdateActiveSlot(node);
            }
            return false;
        }

        private void UpdateActiveSlot(TableEntry<Node> node)
        {
            Graph graph = viewport.graph;
            if (!graph.plugins.TryGet(node.value.plugin, out Plugin plugin)) { return; }

            ListSlice<int> inputs = graph.inputs.GetAtRow(node.row);
            for (int i = 0; i < plugin.inputs; i++)
            {
                Vector2 iPos = node.value.InputPos(i, plugin.inputs);
                TrySetSlot(node.id, iPos, i);
                TrySetWire(iPos, inputs.Consume(), node.id, i);
            }

            TrySetSlot(node.id, node.value.OutputPos(), -1);
        }

        private void TrySetNode(TableEntry<Node> node)
        {
            if (viewport.hover.node != -1) { return; }

            var rect = new Rect(node.value.pos.ToPoint(), Node.Size.ToSize());
            var pos = viewport.From(mouse.current.pos).ToPoint();
            if (rect.Contains(pos))
            {
                viewport.hover.node = node.id;
            }
        }

        private void TrySetSlot(int nodeId, Vector2 pos, int slot)
        {
            float distance = Vector2.Distance(pos, viewport.From(mouse.current.pos));
            if (distance < viewport.hover.slot.distance)
            {
                viewport.hover.slot.target = new Slot(nodeId, slot);
                viewport.hover.slot.distance = distance;
            }
        }

        private void TrySetWire(Vector2 iPos, int oNodeId, int iNodeId, int slot)
        {
            if (!viewport.graph.nodes.TryGet(oNodeId, out Node oNode)) { return; }
            var wire = new Wire(iPos, oNode.OutputPos());
            float distance = wire.Distance(viewport.From(mouse.current.pos));
            HoverInfo hover = viewport.hover;
            if (distance < hover.wire.distance)
            {
                hover.wire.target = new Slot(iNodeId, slot);
                hover.wire.distance = distance;
            }
        }
    }
}
