using Madeline.Frontend.Structure;
using System.Collections.Generic;
using Windows.System;

namespace Madeline.Frontend.Handlers.Graph
{
    internal class DragSelectHandler : IMouseHandler
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
            select.box.start = viewport.From(mouse.current.pos);
            select.box.end = viewport.From(mouse.current.pos);
            dragging = true;
            return true;
        }

        private bool AdvanceSelect()
        {
            if (dragging)
            {
                SelectionInfo select = viewport.selection;
                select.box.end = viewport.From(mouse.current.pos);
            }
            return dragging;
        }

        private bool CommitSelect()
        {
            if (!dragging) { return false; }

            bool ctrl = Utils.IsKeyDown(VirtualKey.Control);
            bool shift = Utils.IsKeyDown(VirtualKey.Shift);
            if (ctrl)
            {
                SubtractNodeSelection();
                SubtractWireSelection();
            }
            else if (shift)
            {
                AddNodeSelection();
                AddWireSelection();
            }
            else
            {
                ReplaceNodeSelection();
                ReplaceWireSelection();
            }

            viewport.selection.candidates.Clear();
            dragging = false;
            viewport.selection.box = Aabb.Zero;
            return true;
        }

        private void SubtractNodeSelection()
        {
            List<int> select = viewport.selection.active.nodes;
            List<int> candidates = viewport.selection.candidates.nodes;
            foreach (int candidate in candidates)
            {
                select.Remove(candidate);
            }
            if (candidates.Contains(viewport.active) && select.Count > 0)
            {
                viewport.active = select[0];
            }
        }

        private void AddNodeSelection()
        {
            List<int> select = viewport.selection.active.nodes;
            List<int> candidates = viewport.selection.candidates.nodes;
            if (candidates.Count > 0)
            {
                viewport.active = candidates[0];
            }
            foreach (int candidate in candidates)
            {
                if (!select.Contains(candidate))
                {
                    select.Add(candidate);
                }
            }
        }

        private void ReplaceNodeSelection()
        {
            List<int> select = viewport.selection.active.nodes;
            List<int> candidates = viewport.selection.candidates.nodes;
            if (candidates.Count > 0)
            {
                viewport.active = candidates[0];
            }
            select.Clear();
            foreach (int candidate in candidates)
            {
                select.Add(candidate);
            }
        }

        private void SubtractWireSelection()
        {
            List<Slot> select = viewport.selection.active.wires;
            List<Slot> candidates = viewport.selection.candidates.wires;
            foreach (Slot candidate in candidates)
            {
                select.Remove(candidate);
            }
        }

        private void AddWireSelection()
        {
            List<Slot> select = viewport.selection.active.wires;
            List<Slot> candidates = viewport.selection.candidates.wires;
            foreach (Slot candidate in candidates)
            {
                if (!select.Contains(candidate))
                {
                    select.Add(candidate);
                }
            }
        }

        private void ReplaceWireSelection()
        {
            List<Slot> select = viewport.selection.active.wires;
            List<Slot> candidates = viewport.selection.candidates.wires;
            select.Clear();
            foreach (Slot candidate in candidates)
            {
                select.Add(candidate);
            }
        }
    }
}
