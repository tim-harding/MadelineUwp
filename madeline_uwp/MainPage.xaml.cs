using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;

namespace madeline_uwp
{
    internal struct Plugin
    {
        public string name;
        public int inputs;
    }

    internal struct Node
    {
        public Vector2 pos;
        public string name;
        public int plugin;
    }

    internal struct Wire
    {
        public int src;
        public int dst;
        public int input;
    }

    internal class Graph
    {
        public List<Plugin> plugins;
        public List<Node> nodes;
        public List<Wire> wires;
    }

    internal struct MouseState
    {
        public bool left;
        public bool middle;
        public bool right;
        public Vector2 pos;
    }

    internal struct Mouse
    {
        public MouseState current;
        public MouseState previous;
    }

    public sealed partial class MainPage : Page
    {
        private Graph graph;
        private readonly Vector2 NODE_SIZE = new Vector2(90, 30f);
        private Mouse mouse;
        private Vector2 transform;
        private float zoom = 1f;

        public MainPage()
        {
            InitializeComponent();
            graph = DefaultGraph();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;

            foreach (Wire wire in graph.wires)
            {
                Node node = graph.nodes[wire.src];
                Plugin plugin = graph.plugins[node.plugin];
                Vector2 src = InputPos(node.pos, wire.input, plugin.inputs);

                node = graph.nodes[wire.dst];
                Vector2 dst = OutputPos(node.pos);

                session.DrawLine(src, dst, Colors.White);
            }

            foreach (Node node in graph.nodes)
            {
                const float ROUNDING = 10f;
                Size size = new Size(NODE_SIZE.X, NODE_SIZE.Y);
                Rect rect = new Rect(VecToPt(ViewportPos(node.pos)), size);
                session.FillRoundedRectangle(rect, ROUNDING, ROUNDING, Color.FromArgb(255, 64, 64, 64));
                session.DrawRoundedRectangle(rect, ROUNDING, ROUNDING, Colors.Black);

                Plugin plugin = graph.plugins[node.plugin];
                for (int i = 0; i < plugin.inputs; i++)
                {
                    DrawNodeIO(session, InputPos(node.pos, i, plugin.inputs));
                }
                DrawNodeIO(session, OutputPos(node.pos));

                Vector2 offset = new Vector2(NODE_SIZE.X + 15f, 0f);
                session.DrawText(node.name, ViewportPos(node.pos + offset), Colors.White);
                offset.Y -= 35f;
                session.DrawText(plugin.name, ViewportPos(node.pos + offset), Colors.Gray);
            }
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void Canvas_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
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
                pos = PtToVec(point.Position),
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

        private Vector2 PtToVec(Point pt)
        {
            return new Vector2
            {
                X = (float)pt.X,
                Y = (float)pt.Y,
            };
        }

        private Point VecToPt(Vector2 vec)
        {
            return new Point
            {
                X = vec.X,
                Y = vec.Y,
            };
        }

        private Vector2 InputPos(Vector2 origin, int input, int inputs)
        {
            const float NODE_SEPARATION = 35f;
            float local = input - (inputs - 1) / 2f;
            Vector2 offset = new Vector2
            {
                X = NODE_SIZE.X / 2f + local * NODE_SEPARATION,
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
            var center =  new Vector2
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
                X = NODE_SIZE.X / 2f,
                Y = NODE_SIZE.Y,
            };
            return ViewportPos(origin) + offset;
        }

        private Graph DefaultGraph()
        {
            List<Plugin> plugins = new List<Plugin>
            {
                new Plugin { name = "Load", inputs = 0 },
                new Plugin { name = "Merge", inputs = 2 },
                new Plugin { name = "Shuffle", inputs = 1 },
            };

            List<Node> nodes = new List<Node>
            {
                new Node { name = "Tree", pos = new Vector2(0, 0), plugin = 0 },
                new Node { name = "Kitty", pos = new Vector2(200, 0), plugin = 0 },
                new Node { name = "Comp", pos = new Vector2(100, 100), plugin = 1 },
                new Node { name = "Swizzle", pos = new Vector2(100, 200), plugin = 2 }
            };

            List<Wire> wires = new List<Wire>
            {
                new Wire { src = 2, dst = 0, input = 0 },
                new Wire { src = 2, dst = 1, input = 1 },
                new Wire { src = 3, dst = 2, input = 0 }
            };

            return new Graph
            {
                plugins = plugins,
                nodes = nodes,
                wires = wires,
            };
        }
    }
}
