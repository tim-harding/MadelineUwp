using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace madeline_uwp
{
    internal struct Node
    {
        public Vector2 pos;
        public string name;
        public int inputs;
    }

    internal struct Wire
    {
        public int src;
        public int dst;
        public int input;
    }

    internal class Graph
    {
        public List<Node> nodes;
        public List<Wire> wires;
    }

    public sealed partial class MainPage : Page
    {
        private Graph graph;
        private readonly Vector2 NODE_SIZE = new Vector2(150f, 50f);

        public MainPage()
        {
            InitializeComponent();
            graph = DefaultGraph();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;

            foreach (Node node in graph.nodes)
            {
                Point point = new Point(node.pos.X, node.pos.Y);
                Rect rect = new Rect(point, new Size(NODE_SIZE.X, NODE_SIZE.Y));
                session.FillRoundedRectangle(rect, 5, 5, Colors.Gray);
                session.DrawRoundedRectangle(rect, 5, 5, Colors.Black);

                for (int i = 0; i < node.inputs; i++)
                {
                    DrawNodeIO(session, InputPos(node, i));
                }
                DrawNodeIO(session, OutputPos(node));
            }

            foreach (Node node in graph.nodes)
            {
                session.DrawText(node.name, node.pos, Colors.White);
            }

            foreach (Wire wire in graph.wires)
            {
                Node srcNode = graph.nodes[wire.src];
                Vector2 src = InputPos(srcNode, wire.input);

                Node dstNode = graph.nodes[wire.dst];
                Vector2 dst = OutputPos(dstNode);

                session.DrawLine(src, dst, Colors.White);
            }
        }

        private Vector2 InputPos(Node node, int input)
        {
            const float NODE_SEPARATION = 35f;
            float local = input - (node.inputs - 1) / 2f;
            Vector2 offset = new Vector2
            {
                X = NODE_SIZE.X / 2f + local * NODE_SEPARATION,
                Y = 0f,
            };
            return node.pos + offset;
        }

        private void DrawNodeIO(CanvasDrawingSession session, Vector2 center)
        {
            session.FillCircle(center, 5, Colors.LightGray);
            session.DrawCircle(center, 5, Colors.Black);
        }

        private Vector2 OutputPos(Node node)
        {
            Vector2 offset = new Vector2
            {
                X = NODE_SIZE.X / 2f,
                Y = NODE_SIZE.Y,
            };
            return node.pos + offset;
        }

        private void Page_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private Graph DefaultGraph()
        {
            List<Node> nodes = new List<Node>
            {
                new Node { name = "Load Tree", pos = new Vector2(0, 0), inputs = 0 },
                new Node { name = "Load Kitty", pos = new Vector2(200, 0), inputs = 0 },
                new Node { name = "Merge", pos = new Vector2(100, 100), inputs = 2 },
                new Node { name = "Swizzle", pos = new Vector2(100, 200), inputs = 1 }
            };

            List<Wire> wires = new List<Wire>
            {
                new Wire { src = 2, dst = 0, input = 0 },
                new Wire { src = 2, dst = 1, input = 1 },
                new Wire { src = 3, dst = 2, input = 0 }
            };

            return new Graph
            {
                nodes = nodes,
                wires = wires,
            };
        }
    }
}
