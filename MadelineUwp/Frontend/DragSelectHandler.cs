using Madeline.Backend;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Madeline.Frontend
{
    internal class DragSelectHandler : MouseHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        private bool dragging = false;

        public DragSelectHandler(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
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
            SelectionInfo select = viewport.selection;
            select.box.start = mouse.current.pos;
            select.box.end = mouse.current.pos;
            dragging = true;
            return true;
        }

        private bool AdvanceSelect()
        {
            if (dragging)
            {
                SelectionInfo select = viewport.selection;
                select.box.end = mouse.current.pos;
                select.candidates.nodes = MatchingNodes();
            }
            return dragging;
        }

        private bool CommitSelect()
        {
            if (!dragging) { return false; }

            bool ctrl = IsDown(VirtualKey.Control);
            bool shift = IsDown(VirtualKey.Shift);

            SelectionInfo select = viewport.selection;
            List<int> nodes = MatchingNodes();
            if (ctrl)
            {
                foreach (int node in nodes)
                {
                    select.active.nodes.Remove(node);
                }
            }
            else if (shift)
            {
                foreach (int node in nodes)
                {
                    if (!select.active.nodes.Contains(node))
                    {
                        select.active.nodes.Add(node);
                    }
                }
            }
            else
            {
                select.active.nodes = nodes;
            }

            select.candidates.Clear();
            dragging = false;
            select.box.start = select.box.end;
            return true;
        }

        private bool IsDown(VirtualKey key)
        {
            CoreWindow window = Window.Current.CoreWindow;
            CoreVirtualKeyStates state = window.GetKeyState(key);
            return state.HasFlag(CoreVirtualKeyStates.Down);
        }

        private List<int> MatchingNodes()
        {
            var nodes = new List<int>();
            foreach (TableEntry<Node> node in viewport.graph.nodes)
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
            var selectRect = viewport.From(viewport.selection.box).ToRect();
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
