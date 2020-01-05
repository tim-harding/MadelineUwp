using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Madeline
{
    public sealed partial class NodeGraph : Page
    {
        private const float NODE_WIDTH = 90f;
        private const float NODE_HEIGHT = 30f;

        private Viewport viewport = new Viewport();
        private NewNodeDialog dialog;
        private Mouse mouse = new Mouse();

        public NodeGraph()
        {
            dialog = new NewNodeDialog(viewport, mouse);
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            Graph graph = viewport.graph;
            foreach ((int nodeId, Node node) in graph.nodes)
            {
                const float ROUNDING = 10f;
                var size = new Size(NODE_WIDTH, NODE_HEIGHT);
                var rect = new Rect(viewport.Into(node.pos).ToPoint(), size);
                session.FillRoundedRectangle(rect, ROUNDING, ROUNDING, Color.FromArgb(255, 64, 64, 64));
                session.DrawRoundedRectangle(rect, ROUNDING, ROUNDING, Colors.Black);

                Plugin plugin = graph.plugins.Get(node.plugin);
                ListSlice<int> inputs = graph.inputs.Get(nodeId);
                for (int i = 0; i < plugin.inputs; i++)
                {
                    Vector2 inPos = InputPos(node.pos, i, plugin.inputs);
                    int inputNodeId = inputs.Consume();
                    if (graph.nodes.TryGet(inputNodeId, out Node srcNode))
                    {
                        Vector2 outPos = OutputPos(srcNode.pos);
                        session.DrawLine(inPos, outPos, Colors.White);
                    }
                    DrawNodeIO(session, inPos);
                }
                DrawNodeIO(session, OutputPos(node.pos));

                var offset = new Vector2(NODE_WIDTH + 15f, 0f);
                session.DrawText(node.name, viewport.Into(node.pos + offset), Colors.White);
                offset.Y -= 25f;
                session.DrawText(plugin.name, viewport.Into(node.pos + offset), Colors.Gray);
            }
            dialog.Draw(session);
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

        private void Update(object sender, PointerRoutedEventArgs e)
        {
            UpdateMouse(e);
            dialog.HandleMouse(mouse);
            UpdateView();
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

        private void UpdateView()
        {
            bool dragging = mouse.current.left && mouse.previous.left;
            if (mouse.current.Equals(mouse.previous) || !dragging)
            {
                return;
            }
            Vector2 delta = mouse.current.pos - mouse.previous.pos;
            viewport.Move(delta);
            canvas.Invalidate();
        }

        private Vector2 InputPos(Vector2 origin, int input, int inputs)
        {
            const float NODE_SEPARATION = 35f;
            float local = input - (inputs - 1) / 2f;
            var offset = new Vector2
            {
                X = NODE_WIDTH / 2f + local * NODE_SEPARATION,
                Y = 0f,
            };
            return viewport.Into(origin) + offset;
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center)
        {
            session.FillCircle(center, 5, Colors.LightGray);
            session.DrawCircle(center, 5, Colors.Black);
        }

        private Vector2 OutputPos(Vector2 origin)
        {
            var offset = new Vector2
            {
                X = NODE_WIDTH / 2f,
                Y = NODE_HEIGHT,
            };
            return viewport.Into(origin) + offset;
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            dialog.HandleKeyboard(args);
            canvas.Invalidate();
        }
    }
}
