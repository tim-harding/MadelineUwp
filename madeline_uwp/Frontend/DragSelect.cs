using Madeline.Backend;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend
{
    internal class DragSelect : MouseHandler
    {
        private Mouse mouse;
        private Viewport viewport;

        private bool dragging = false;

        public DragSelect(Mouse mouse, Viewport viewport)
        {
            this.mouse = mouse;
            this.viewport = viewport;
        }

        public bool HandleMouse()
        {
            switch (mouse.Left)
            {
                case MouseButton.Down:
                    return BeginSelect();

                case MouseButton.Dragging:
                    return AdvanceSelect();

                case MouseButton.Up:
                    return CommitSelect();
            }
            return false;
        }

        private bool BeginSelect()
        {
            if (viewport.hover.node != -1)
            {
                return false;
            }
            viewport.selectBoxStart = mouse.current.pos;
            viewport.selectBoxEnd = mouse.current.pos;
            dragging = true;
            return true;
        }

        private bool AdvanceSelect()
        {
            if (!dragging)
            {
                return false;
            }
            viewport.selectBoxEnd = mouse.current.pos;
            return true;
        }

        private bool CommitSelect()
        {
            if (!dragging)
            {
                return false;
            }
            dragging = false;
            viewport.selectBoxStart = viewport.selectBoxEnd;
            return true;
        }

        private List<int> MatchingNodes()
        {
            var nodes = new List<int>();
            foreach (TableRow<Node> node in viewport.graph.nodes)
            {
                if (Includes(node.value.pos))
                {
                    nodes.Add(node.id);
                }
            }
            return nodes;
        }

        private List<Slot> MatchingSlots()
        {
            var slots = new List<Slot>();
            return slots;
        }

        private List<Slot> MatchingWires()
        {
            var wires = new List<Slot>();
            return wires;
        }

        private bool Includes(Vector2 nodePos)
        {
            var selectRect = new Rect(viewport.selectBoxStart.ToPoint(), viewport.selectBoxEnd.ToPoint());
            var nodeRect = new Rect(nodePos.ToPoint(), Node.Size.ToSize());
            selectRect.Intersect(nodeRect);
            return !selectRect.IsEmpty;
        }

        private bool Intersect(Rect aabb, Vector2 start, Vector2 end)
        {
            Vector2 dir = end - start;
            dir /= dir.Length();

            var min = new Vector2((float)aabb.Left, (float)aabb.Bottom);
            var max = new Vector2((float)aabb.Right, (float)aabb.Top);

            Vector2 tmin = (min - start) / dir;
            Vector2 tmax = (max - start) / dir;

            if (tmin.X > tmax.X)
            {
                Swap(ref tmin.X, ref tmax.X);
            }

            if (tmin.Y > tmax.Y)
            {
                Swap(ref tmin.Y, ref tmax.Y);
            }

            return tmin.X > tmax.Y || tmin.Y > tmax.X;
        }

        private void Swap(ref float lhs, ref float rhs)
        {
            float tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
