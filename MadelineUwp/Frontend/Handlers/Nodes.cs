using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Madeline.Frontend.Handlers
{
    internal class Nodes : IMouseHandler, IKeypressHandler
    {
        private Viewport viewport;
        private Mouse mouse;

        private Vector2 cursorStart;
        private Vector2 nodeStart;
        private int clickedNode = -1;
        private bool dragStarted;

        public Nodes(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public bool HandleMouse()
        {
            return HandleLeftButton() || HandleMiddleButton() || HandleRightButton();
        }

        public bool HandleKeypress(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Delete:
                case VirtualKey.Back:
                    DeleteNodes();
                    return true;

                case VirtualKey.R:
                    viewport.viewing = viewport.active;
                    break;

                case VirtualKey.Q:
                    DisableNodes();
                    break;

                case VirtualKey.Z:
                    ShiftHistory();
                    break;
            }
            return false;
        }

        private bool HandleMiddleButton()
        {
            switch (mouse.Middle)
            {
                case MouseButton.Dragging:
                    viewport.Move(mouse.Delta);
                    return true;
            }
            return false;
        }

        private bool HandleRightButton()
        {
            switch (mouse.Right)
            {
                case MouseButton.Down:
                    cursorStart = mouse.current.pos;
                    return true;

                case MouseButton.Dragging:
                    int delta = (int)(mouse.Delta.X) * 3;
                    viewport.ZoomAround(cursorStart, delta);
                    return true;
            }
            return false;
        }

        private bool HandleLeftButton()
        {
            switch (mouse.Left)
            {
                case MouseButton.Down:
                    return BeginLmbInteration();

                case MouseButton.Dragging:
                    return AdvanceLmbInteration();

                case MouseButton.Up:
                    return CommitLmbInteration();
            }
            return false;
        }

        private bool BeginLmbInteration()
        {
            cursorStart = mouse.current.pos;
            clickedNode = -1;
            int hover = viewport.hover.node.id;
            bool hasHover = hover > -1;
            if (hasHover)
            {
                clickedNode = hover;
                dragStarted = false;

                // Should always succeed if we have hover
                if (viewport.graph.nodes.TryGet(hover, out Node node))
                {
                    nodeStart = node.pos;
                }
            }
            return hasHover;
        }

        private bool AdvanceLmbInteration()
        {
            bool handling = clickedNode > -1;
            if (handling)
            {
                CheckDragStarted();
                if (dragStarted)
                {
                    DragNode();
                }
            }
            return handling;
        }

        private bool CommitLmbInteration()
        {
            bool handling = clickedNode > -1;
            if (handling && !dragStarted)
            {
                switch (viewport.hover.node.state)
                {
                    case Structure.NodeHover.State.Body:
                        bool ctrl = IsKeyDown(VirtualKey.Control);
                        bool shift = IsKeyDown(VirtualKey.Shift);
                        List<int> nodes = viewport.selection.active.nodes;
                        if (ctrl)
                        {
                            nodes.Remove(clickedNode);
                            if (clickedNode == viewport.active && nodes.Count > 0)
                            {
                                viewport.active = nodes[0];
                            }
                        }
                        else if (shift)
                        {
                            viewport.active = clickedNode;
                            if (!nodes.Contains(clickedNode))
                            {
                                nodes.Add(clickedNode);
                            }
                        }
                        else
                        {
                            viewport.active = clickedNode;
                            nodes.Clear();
                            nodes.Add(clickedNode);
                        }
                        break;

                    case Structure.NodeHover.State.Disable:
                        if (viewport.graph.nodes.TryGet(viewport.hover.node.id, out Node node))
                        {
                            node.enabled = !node.enabled;
                        }
                        break;

                    case Structure.NodeHover.State.Viewing:
                        viewport.viewing = viewport.hover.node.id;
                        break;
                }
            }
            clickedNode = -1;
            return handling;
        }

        private void CheckDragStarted()
        {
            const float DRAG_START = 16f;
            viewport.hover.node.id = clickedNode;
            Vector2 delta = mouse.current.pos - cursorStart;
            dragStarted |= delta.LengthSquared() > DRAG_START;
        }

        private void DragNode()
        {
            Graph graph = viewport.graph;
            Table<Node> nodes = graph.nodes;
            int active = viewport.hover.node.id;
            if (graph.nodes.TryGet(active, out Node node))
            {
                Vector2 mouseDelta = mouse.current.pos - cursorStart;
                mouseDelta /= viewport.zoom;
                Vector2 endPos = nodeStart + mouseDelta;
                endPos += SnapDelta(endPos, active);
                node.pos = endPos;
            }
        }

        private Vector2 SnapDelta(Vector2 pos, int nodeId)
        {
            Vector2 snap = Vector2.One * float.MaxValue;
            foreach (TableEntry<Node> node in viewport.graph.nodes)
            {
                if (node.id == nodeId) { continue; }

                Vector2 delta = node.value.pos - pos;
                snap.Y = Math.Abs(delta.Y) < Math.Abs(snap.Y) ? delta.Y : snap.Y;
                snap.X = Math.Abs(delta.X) < Math.Abs(snap.X) ? delta.X : snap.X;
            }

            float SNAP_LIMIT = 12f;
            snap.X = Math.Abs(snap.X) < SNAP_LIMIT ? snap.X : 0f;
            snap.Y = Math.Abs(snap.Y) < SNAP_LIMIT ? snap.Y : 0f;
            return snap;
        }

        private void DisableNodes()
        {
            foreach (int nodeId in viewport.selection.active.nodes)
            {
                if (viewport.graph.nodes.TryGet(nodeId, out Node node))
                {
                    node.enabled = !node.enabled;
                }
            }
        }

        private void ShiftHistory()
        {
            bool ctrl = IsKeyDown(VirtualKey.Control);
            bool shift = IsKeyDown(VirtualKey.Shift);
            History history = viewport.history;
            if (ctrl && shift)
            {
                history.Redo();
            }
            else if (ctrl)
            {
                history.Undo();
            }
        }

        private bool IsKeyDown(VirtualKey key)
        {
            CoreWindow window = Window.Current.CoreWindow;
            CoreVirtualKeyStates state = window.GetKeyState(key);
            return state.HasFlag(CoreVirtualKeyStates.Down);
        }

        private void DeleteNodes()
        {
            List<int> select = viewport.selection.active.nodes;
            if (select.Count == 0) { return; }

            var nodes = new Node[select.Count];
            int[] ids = new int[select.Count];
            for (int i = 0; i < select.Count; i++)
            {
                int id = select[i];
                ids[i] = id;
                if (viewport.graph.nodes.TryGet(id, out Node node))
                {
                    nodes[i] = node;
                }
            }
            viewport.history.SubmitChange(new HistoricEvents.DeleteNodes(ids, nodes));
        }
    }
}
