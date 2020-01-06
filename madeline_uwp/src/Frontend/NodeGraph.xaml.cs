using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Madeline
{
    public sealed partial class NodeGraph : Page
    {
        private Viewport viewport = new Viewport();
        private NodeCreationDialog dialog;
        private NodesDrawer nodesDrawer;
        private Mouse mouse = new Mouse();

        private Vector2 start;
        private int clickedNode = -1;
        private bool dragStarted;

        public NodeGraph()
        {
            dialog = new NodeCreationDialog(viewport, mouse);
            nodesDrawer = new NodesDrawer(viewport);
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            nodesDrawer.Draw(session);
            dialog.drawer.Draw(session);
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void HandleScroll(object sender, PointerRoutedEventArgs e)
        {
            int wheel = e.GetCurrentPoint(canvas).Properties.MouseWheelDelta;
            viewport.ZoomAround(mouse.current.pos, wheel);
            canvas.Invalidate();
        }

        private void HandleMouse(object sender, PointerRoutedEventArgs e)
        {
            UpdateMouse(e);
            UpdateActiveNode();

            switch (mouse.Middle)
            {
                case MouseButton.Dragging:
                    viewport.Move(mouse.Delta);
                    break;
            }

            switch (mouse.Right)
            {
                case MouseButton.Down:
                    start = mouse.current.pos;
                    break;

                case MouseButton.Dragging:
                    int delta = (int)(mouse.Delta.X) * 3;
                    viewport.ZoomAround(start, delta);
                    break;
            }

            switch (mouse.Left)
            {
                case MouseButton.Down:
                    start = mouse.current.pos;
                    clickedNode = -1;
                    if (viewport.graph.hover > -1)
                    {
                        clickedNode = viewport.graph.hover;
                        dragStarted = false;
                    }
                    break;

                case MouseButton.Dragging:
                    const float DRAG_START = 16f;
                    viewport.graph.hover = clickedNode;
                    dragStarted |= (mouse.current.pos - start).LengthSquared() > DRAG_START;
                    if (!dragStarted)
                    {
                        break;
                    }

                    Graph graph = viewport.graph;
                    Table<Node> nodes = graph.nodes;
                    int active = graph.hover;
                    if (nodes.TryGet(active, out Node node))
                    {
                        node.pos += mouse.Delta / viewport.zoom;
                        nodes.Update(active, node);
                    }
                    break;

                case MouseButton.Up:
                    viewport.graph.hover = clickedNode;
                    if (!dragStarted)
                    {
                        viewport.graph.active = viewport.graph.hover;
                    }
                    break;
            }

            dialog.HandleMouse(mouse);
            canvas.Invalidate();
        }

        private void UpdateMouse(PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(canvas);
            PointerPointProperties props = point.Properties;
            mouse.previous = mouse.current;
            mouse.current = new MouseState
            {
                left = props.IsLeftButtonPressed,
                right = props.IsRightButtonPressed,
                middle = props.IsMiddleButtonPressed,
                pos = point.Position.ToVector2(),
            };
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            canvas.Invalidate();
            if (dialog.HandleKeyboard(args))
            {
                return;
            }

            switch (args.VirtualKey)
            {
                case VirtualKey.Delete:
                case VirtualKey.Back:
                    Graph graph = viewport.graph;
                    graph.DeleteNode(graph.active);
                    break;
            }
        }

        private void UpdateActiveNode()
        {
            viewport.graph.hover = -1;
            var pos = viewport.From(mouse.current.pos).ToPoint();
            foreach ((int id, Node node) pair in viewport.graph.nodes)
            {
                var size = new Size(NodesDrawer.NODE_WIDTH, NodesDrawer.NODE_HEIGHT);
                var rect = new Rect(pair.node.pos.ToPoint(), size);
                if (rect.Contains(pos))
                {
                    viewport.graph.hover = pair.id;
                    return;
                }
            }
        }
    }
}
