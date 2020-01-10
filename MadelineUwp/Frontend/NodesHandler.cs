using Madeline.Backend;
using System;
using System.Numerics;
using Windows.System;

namespace Madeline.Frontend
{
    internal class NodesHandler : MouseHandler, KeypressHandler
    {
        private struct SnapPair
        {
            public bool vSnap;
            public bool hSnap;
            public Vector2 delta;
        }

        private Viewport viewport;
        private Mouse mouse;

        private Vector2 start;
        private int clickedNode = -1;
        private bool dragStarted;

        public NodesHandler(Viewport viewport, Mouse mouse)
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
                    viewport.graph.DeleteNode(viewport.selection.ActiveNode);
                    return true;

                case VirtualKey.R:
                    viewport.viewing = viewport.selection.ActiveNode;
                    break;

                case VirtualKey.Q:
                    DisableNodes();
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
                    start = mouse.current.pos;
                    return true;

                case MouseButton.Dragging:
                    int delta = (int)(mouse.Delta.X) * 3;
                    viewport.ZoomAround(start, delta);
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
            start = mouse.current.pos;
            clickedNode = -1;
            int hover = viewport.hover.node;
            bool hasHover = hover > -1;
            if (hasHover)
            {
                clickedNode = hover;
                dragStarted = false;
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
            if (handling)
            {
                if (!dragStarted)
                {
                    viewport.selection.ActiveNode = clickedNode;
                }
                clickedNode = -1;
            }
            return handling;
        }

        private void CheckDragStarted()
        {
            const float DRAG_START = 16f;
            viewport.hover.node = clickedNode;
            Vector2 delta = mouse.current.pos - start;
            dragStarted |= delta.LengthSquared() > DRAG_START;
        }

        private void DragNode()
        {
            Graph graph = viewport.graph;
            Table<Node> nodes = graph.nodes;
            int active = viewport.hover.node;
            if (graph.nodes.TryGetRowForId(active, out int row))
            {
                Node node = nodes.GetAtRow(row);

                node.pos += mouse.Delta / viewport.zoom;
                SnapPair snap = FindSnapPair(node.pos, active);
                if (snap.hSnap)
                {
                    node.pos.X += snap.delta.X;
                }
                if (snap.vSnap)
                {
                    node.pos.Y += snap.delta.Y;
                }

                nodes.UpdateAtRow(row, node);
            }
        }

        private SnapPair FindSnapPair(Vector2 pos, int nodeId)
        {
            float SNAP_LIMIT = 8f;
            var pair = new SnapPair();
            pair.delta.X = float.MaxValue;
            pair.delta.Y = float.MaxValue;
            foreach (TableEntry<Node> node in viewport.graph.nodes)
            {
                if (node.id == nodeId) { continue; }

                Vector2 delta = node.value.pos - pos;
                var abs = new Vector2(Math.Abs(delta.X), Math.Abs(delta.Y));
                if (abs.Y < SNAP_LIMIT && abs.Y < Math.Abs(pair.delta.Y))
                {
                    pair.delta.Y = delta.Y;
                    pair.vSnap = true;
                }
                if (abs.X < SNAP_LIMIT && abs.X < Math.Abs(pair.delta.X))
                {
                    pair.delta.X = delta.X;
                    pair.hSnap = true;
                }
            }
            return pair;
        }

        private void DisableNodes()
        {
            Graph graph = viewport.graph;
            if (graph.nodes.TryGetRowForId(viewport.selection.ActiveNode, out int row))
            {
                Node node = graph.nodes.GetAtRow(row);
                node.enabled = !node.enabled;
                graph.nodes.UpdateAtRow(row, node);
            }
        }
    }
}
