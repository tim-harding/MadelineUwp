﻿using System.Collections.Generic;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Madeline.Frontend.Handlers
{
    internal class DragSelect : IMouseHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        private bool dragging = false;

        public DragSelect(Viewport viewport, Mouse mouse)
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

            CommitNodes();
            CommitWires();

            viewport.selection.candidates.Clear();
            dragging = false;
            viewport.selection.box = Aabb.Zero;
            return true;
        }

        private void CommitNodes()
        {
            List<int> select = viewport.selection.active.nodes;
            List<int> candidates = viewport.selection.candidates.nodes;
            bool ctrl = IsDown(VirtualKey.Control);
            bool shift = IsDown(VirtualKey.Shift);
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
        }

        private void CommitWires()
        {
            List<Slot> select = viewport.selection.active.wires;
            List<Slot> candidates = viewport.selection.candidates.wires;
            bool ctrl = IsDown(VirtualKey.Control);
            bool shift = IsDown(VirtualKey.Shift);
            if (ctrl)
            {
                foreach (Slot candidate in candidates)
                {
                    select.Remove(candidate);
                }
            }
            else if (shift)
            {
                foreach (Slot candidate in candidates)
                {
                    if (!select.Contains(candidate))
                    {
                        select.Add(candidate);
                    }
                }
            }
            else
            {
                select.Clear();
                foreach (Slot candidate in candidates)
                {
                    select.Add(candidate);
                }
            }
        }

        private bool IsDown(VirtualKey key)
        {
            CoreWindow window = Window.Current.CoreWindow;
            CoreVirtualKeyStates state = window.GetKeyState(key);
            return state.HasFlag(CoreVirtualKeyStates.Down);
        }
    }
}