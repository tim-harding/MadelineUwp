using Madeline.Backend;
using System.Numerics;
using Windows.System;

namespace Madeline.Frontend
{
    internal class NodesHandler : MouseHandler, KeypressHandler
    {
        private Mouse mouse;
        private Viewport viewport;

        private Vector2 start;
        private int clickedNode = -1;
        private bool dragStarted;

        public NodesHandler(Mouse mouse, Viewport viewport)
        {
            this.mouse = mouse;
            this.viewport = viewport;
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
                    viewport.graph.DeleteNode(viewport.active.node);
                    return true;

                case VirtualKey.R:
                    viewport.viewing = viewport.active.node;
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
                    viewport.active.node = clickedNode;
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
            if (nodes.TryGet(active, out Node node))
            {
                node.pos += mouse.Delta / viewport.zoom;
                nodes.Update(active, node);
            }
        }

        private void DisableNodes()
        {
            if (viewport.graph.nodes.TryGet(viewport.active.node, out Node node))
            {
                node.enabled = !node.enabled;
                viewport.graph.nodes.Update(viewport.active.node, node);
            }
        }
    }
}
