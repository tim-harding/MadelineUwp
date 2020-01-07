using Madeline.Backend;
using System.Numerics;
using Windows.System;

namespace Madeline.Frontend
{
    internal class NodeInteraction : MouseHandler, KeypressHandler
    {
        private Mouse mouse;
        private Viewport viewport;

        private Vector2 start;
        private int clickedNode = -1;
        private bool dragStarted;

        public NodeInteraction(Mouse mouse, Viewport viewport)
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
                    BeginLmbInteration();
                    return true;

                case MouseButton.Dragging:
                    AdvanceLmbInteration();
                    return true;

                case MouseButton.Up:
                    CommitLmbInteration();
                    return true;
            }
            return false;
        }

        private void BeginLmbInteration()
        {
            start = mouse.current.pos;
            clickedNode = -1;
            int hover = viewport.hover.node;
            if (hover > -1)
            {
                clickedNode = hover;
                dragStarted = false;
            }
        }

        private void AdvanceLmbInteration()
        {
            CheckDragStarted();
            if (dragStarted)
            {
                DragNode();
            }
        }

        private void CommitLmbInteration()
        {
            viewport.hover.node = clickedNode;
            if (!dragStarted)
            {
                viewport.active.node = viewport.hover.node;
            }
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
    }
}
