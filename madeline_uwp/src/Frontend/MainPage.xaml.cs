using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Core;

namespace Madeline
{
    public sealed partial class MainPage : Page
    {
        private const float NODE_WIDTH = 90f;
        private const float NODE_HEIGHT = 30f;

        private Graph graph = SampleData.DefaultGraph();
        private Mouse mouse = new Mouse();
        private Vector2 transform;
        private float zoom = 1f;
        private NewNodeDialog dialog;

        public MainPage()
        {
            dialog = new NewNodeDialog(graph, mouse);
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;

            foreach ((int nodeId, Node node) in graph.nodes)
            {
                const float ROUNDING = 10f;
                Size size = new Size(NODE_WIDTH, NODE_HEIGHT);
                Rect rect = new Rect(ViewportPos(node.pos).ToPoint(), size);
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

                Vector2 offset = new Vector2(NODE_WIDTH + 15f, 0f);
                session.DrawText(node.name, ViewportPos(node.pos + offset), Colors.White);
                offset.Y -= 35f;
                session.DrawText(plugin.name, ViewportPos(node.pos + offset), Colors.Gray);
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
            float delta = (float)Math.Pow(1.001, wheel);
            zoom *= delta;
            transform = GraphPos(ViewportPos(transform) - mouse.current.pos * (delta - 1f));
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
            if (mouse.current.Equals(mouse.previous))
            {
                return;
            }
            if (mouse.current.left && mouse.previous.left)
            {
                Vector2 delta = mouse.current.pos - mouse.previous.pos;
                transform += delta * 1f / zoom;
            }
            canvas.Invalidate();
        }


        private Vector2 InputPos(Vector2 origin, int input, int inputs)
        {
            const float NODE_SEPARATION = 35f;
            float local = input - (inputs - 1) / 2f;
            Vector2 offset = new Vector2
            {
                X = NODE_WIDTH / 2f + local * NODE_SEPARATION,
                Y = 0f,
            };
            return ViewportPos(origin) + offset;
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center)
        {
            session.FillCircle(center, 5, Colors.LightGray);
            session.DrawCircle(center, 5, Colors.Black);
        }

        private Vector2 ViewportPos(Vector2 pos)
        {
            return (pos + transform) * zoom;
        }

        private Vector2 GraphPos(Vector2 pos)
        {
            return pos / zoom - transform;
        }

        private Vector2 ViewportCenter()
        {
            Vector2 center = new Vector2
            {
                X = (float)canvas.Size.Width / 2f,
                Y = (float)canvas.Size.Height / 2f,
            };
            return center;
        }

        private Vector2 OutputPos(Vector2 origin)
        {
            Vector2 offset = new Vector2
            {
                X = NODE_WIDTH / 2f,
                Y = NODE_HEIGHT,
            };
            return ViewportPos(origin) + offset;
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            dialog.HandleKeyboard(args, graph);
            canvas.Invalidate();
        }
    }
}
