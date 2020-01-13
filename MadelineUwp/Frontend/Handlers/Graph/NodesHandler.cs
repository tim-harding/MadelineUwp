using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.System;

namespace Madeline.Frontend.Handlers.Graph
{
    internal class NodesHandler : IInputHandler
    {
        private Viewport viewport;

        private Vector2 cursorStart;
        private Vector2 nodeStart;
        private int clickedNode = -1;
        private bool dragStarted;

        public NodesHandler(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public bool HandleScroll(int delta)
        {
            viewport.ZoomAround(Mouse.current.pos, delta);
            return true;
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
            switch (Mouse.Middle)
            {
                case Mouse.Button.Dragging:
                    viewport.Move(Mouse.Delta);
                    return true;
            }
            return false;
        }

        private bool HandleRightButton()
        {
            switch (Mouse.Right)
            {
                case Mouse.Button.Down:
                    cursorStart = Mouse.current.pos;
                    return true;

                case Mouse.Button.Dragging:
                    int delta = (int)(Mouse.Delta.X) * 3;
                    viewport.ZoomAround(cursorStart, delta);
                    return true;
            }
            return false;
        }

        private bool HandleLeftButton()
        {
            switch (Mouse.Left)
            {
                case Mouse.Button.Down:
                    return BeginLmbInteration();

                case Mouse.Button.Dragging:
                    return AdvanceLmbInteration();

                case Mouse.Button.Up:
                    return CommitLmbInteration();
            }
            return false;
        }

        private bool BeginLmbInteration()
        {
            cursorStart = Mouse.current.pos;
            clickedNode = -1;
            int hover = viewport.hover.node.id;
            bool hasHover = hover > -1;
            if (hasHover)
            {
                clickedNode = hover;
                dragStarted = false;

                if (Globals.graph.nodes.TryGet(hover, out Node node))
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
                        ModifyNodeSelection();
                        break;

                    case Structure.NodeHover.State.Disable:
                        DisableNode();
                        break;

                    case Structure.NodeHover.State.Viewing:
                        viewport.viewing = viewport.hover.node.id;
                        break;
                }
            }
            clickedNode = -1;
            return handling;
        }

        private void ModifyNodeSelection()
        {
            bool ctrl = Utils.IsKeyDown(VirtualKey.Control);
            bool shift = Utils.IsKeyDown(VirtualKey.Shift);
            if (ctrl)
            {
                DeselectNode();
            }
            else if (shift)
            {
                AddNodeToSelection();
            }
            else
            {
                ReplaceNodeSelection();
            }
        }

        private void DeselectNode()
        {
            List<int> nodes = viewport.selection.active.nodes;
            nodes.Remove(clickedNode);
            if (clickedNode == viewport.active && nodes.Count > 0)
            {
                viewport.active = nodes[0];
            }
        }

        private void AddNodeToSelection()
        {
            List<int> nodes = viewport.selection.active.nodes;
            viewport.active = clickedNode;
            if (!nodes.Contains(clickedNode))
            {
                nodes.Add(clickedNode);
            }
        }

        private void ReplaceNodeSelection()
        {
            List<int> nodes = viewport.selection.active.nodes;
            viewport.active = clickedNode;
            nodes.Clear();
            nodes.Add(clickedNode);
        }

        private void DisableNode()
        {
            if (Globals.graph.nodes.TryGet(viewport.hover.node.id, out Node node))
            {
                node.enabled = !node.enabled;
            }
        }

        private void CheckDragStarted()
        {
            const float DRAG_START = 16f;
            viewport.hover.node.id = clickedNode;
            Vector2 delta = Mouse.current.pos - cursorStart;
            dragStarted |= delta.LengthSquared() > DRAG_START;
        }

        // TODO: Should drag all selected nodes at once
        private void DragNode()
        {
            NodeGraph graph = Globals.graph;
            Table<Node> nodes = graph.nodes;
            int active = viewport.hover.node.id;
            if (graph.nodes.TryGet(active, out Node node))
            {
                Vector2 mouseDelta = Mouse.current.pos - cursorStart;
                mouseDelta /= viewport.zoom;
                Vector2 endPos = nodeStart + mouseDelta;
                endPos += SnapDelta(endPos, active);
                node.pos = endPos;
            }
        }

        private Vector2 SnapDelta(Vector2 pos, int nodeId)
        {
            Vector2 snap = Vector2.One * float.MaxValue;
            foreach (TableEntry<Node> node in Globals.graph.nodes)
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
                if (Globals.graph.nodes.TryGet(nodeId, out Node node))
                {
                    node.enabled = !node.enabled;
                }
            }
        }

        private void ShiftHistory()
        {
            bool ctrl = Utils.IsKeyDown(VirtualKey.Control);
            bool shift = Utils.IsKeyDown(VirtualKey.Shift);
            History history = Globals.history;
            if (ctrl && shift)
            {
                history.Redo();
            }
            else if (ctrl)
            {
                history.Undo();
            }
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
                if (Globals.graph.nodes.TryGet(id, out Node node))
                {
                    nodes[i] = node;
                }
            }
            Globals.history.SubmitChange(new HistoricEvents.DeleteNodes(ids, nodes));
        }
    }
}
