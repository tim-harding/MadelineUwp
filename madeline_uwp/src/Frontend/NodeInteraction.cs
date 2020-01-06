using Madeline.Backend;
using System.Numerics;
using Windows.System;

namespace Madeline
{
    internal class NodeInteraction
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
                    Graph graph = viewport.graph;
                    graph.DeleteNode(graph.active);
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
            if (viewport.graph.hover > -1)
            {
                clickedNode = viewport.graph.hover;
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
            viewport.graph.hover = clickedNode;
            if (!dragStarted)
            {
                viewport.graph.active = viewport.graph.hover;
            }
        }

        private void CheckDragStarted()
        {
            const float DRAG_START = 16f;
            viewport.graph.hover = clickedNode;
            Vector2 delta = mouse.current.pos - start;
            dragStarted |= delta.LengthSquared() > DRAG_START;
        }

        private void DragNode()
        {
            Graph graph = viewport.graph;
            Table<Node> nodes = graph.nodes;
            int active = graph.hover;
            if (nodes.TryGet(active, out Node node))
            {
                node.pos += mouse.Delta / viewport.zoom;
                nodes.Update(active, node);
            }
        }
    }
}
