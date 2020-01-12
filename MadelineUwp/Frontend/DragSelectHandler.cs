using System.Collections.Generic;
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

            bool ctrl = IsDown(VirtualKey.Control);
            bool shift = IsDown(VirtualKey.Shift);

            List<int> select = viewport.selection.active.nodes;
            List<int> candidates = viewport.selection.candidates.nodes;

            if (ctrl)
            {
                foreach (int candidate in candidates)
                {
                    select.Remove(candidate);
                }
                if (candidates.Contains(viewport.active) && select.Count > 0)
                {
                    viewport.active = select[0];
                }
            }
            else if (shift)
            {
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
            else
            {
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

            viewport.selection.candidates.Clear();
            dragging = false;
            viewport.selection.box = Aabb.Zero;
            return true;
        }

        private bool IsDown(VirtualKey key)
        {
            CoreWindow window = Window.Current.CoreWindow;
            CoreVirtualKeyStates state = window.GetKeyState(key);
            return state.HasFlag(CoreVirtualKeyStates.Down);
        }
    }
}
