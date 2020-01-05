using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;

namespace Madeline
{
    internal class NewNodeDialog
    {
        private const int WIDTH = 150;
        private const int HEIGHT = 600;
        private const int LEADING = 25;

        private Mouse mouse;
        private Graph graph;
        private List<(int id, Plugin plugin)> found = new List<(int, Plugin)>();
        private string query = "";
        private int selection = 0;
        private int failPoint;
        private bool display;
        private Vector2 origin;

        public NewNodeDialog(Graph graph, Mouse mouse)
        {
            this.graph = graph;
            this.mouse = mouse;
        }

        public void Draw(CanvasDrawingSession session)
        {
            if (!display)
            {
                return;
            }

            var size = new Size(WIDTH, HEIGHT);
            var rect = new Rect(origin.ToPoint(), size);
            session.FillRectangle(rect, Color.FromArgb(255, 48, 48, 48));

            var margin = new Vector2(6f, 0f);
            Vector2 pos = origin;
            pos.Y += LEADING;
            session.DrawText(query, origin + margin, Colors.White);

            if (failPoint != -1)
            {
                string valid = query.Substring(0, failPoint);
                var format = new CanvasTextFormat();
                var layout = new CanvasTextLayout(session.Device, valid, format, WIDTH, HEIGHT);
                session.DrawTextLayout(layout, origin + margin, Colors.White);
                Rect bounds = layout.LayoutBounds;
                string invalid = query.Substring(failPoint);
                var offset = new Vector2((float)bounds.Width, 0f);
                session.DrawText(invalid, origin + margin + offset, Colors.Red);
            }
            else
            {
                session.DrawText(query, origin + margin, Colors.White);
            }

            session.DrawLine(pos, pos + new Vector2(WIDTH, 0f), Colors.White);

            if (selection > -1)
            {
                var offset = new Vector2(0f, LEADING * selection);
                size = new Size(WIDTH, LEADING);
                rect = new Rect((pos + offset).ToPoint(), size);
                session.FillRectangle(rect, Color.FromArgb(255, 64, 64, 64));
            }


            int count = Math.Min(found.Count, HEIGHT / LEADING);
            for (int i = 0; i < count; i++)
            {
                session.DrawText(found[i].plugin.name, pos + margin, Colors.White);
                pos.Y += LEADING;
            }
        }

        public void HandleKeyboard(KeyEventArgs e, Graph graph)
        {
            if (!e.KeyStatus.WasKeyDown)
            {
                return;
            }

            VirtualKey key = e.VirtualKey;
            switch (key)
            {
                case VirtualKey.Tab:
                    if (display)
                    {
                        Hide();
                    }
                    else
                    {
                        Show();
                    }
                    break;

                case VirtualKey.Escape:
                    Hide();
                    break;

                case VirtualKey.Back:
                    if (query.Length > 0)
                    {
                        query = query.Remove(query.Length - 1);
                    }
                    UpdateFound();
                    break;

                case VirtualKey.Down:
                    selection += 1;
                    UpdateSeletion();
                    break;

                case VirtualKey.Up:
                    selection -= 1;
                    UpdateSeletion();
                    break;

                case VirtualKey.Enter:
                    if (selection > -1 && selection < found.Count)
                    {
                        (int id, Plugin plugin) pair = found[selection];
                        graph.InsertNode(origin, pair.id);
                    }
                    Hide();
                    break;

                default:
                    char ascii = (char)key;
                    bool space = ascii == 32;
                    bool upper = ascii > 64 && ascii < 91;
                    bool lower = ascii > 96 && ascii < 123;
                    bool wanted = space || upper || lower;
                    if (!wanted)
                    {
                        break;
                    }
                    query += char.ToLower(ascii);
                    UpdateFound();
                    break;
            }
        }

        private void UpdateFound()
        {
            int previousCount = found.Count;
            foreach ((int id, Plugin plugin) pair in graph.plugins)
            {
                if (query.Length == 0 || pair.plugin.name.Contains(query))
                {
                    found.Add(pair);
                }
            }

            bool searchFailure = found.Count == previousCount;
            if (searchFailure)
            {
                failPoint = failPoint > -1 ? failPoint : query.Length - 1;
            }
            else
            {
                found.RemoveRange(0, previousCount);
                failPoint = -1;
            }

            UpdateSeletion();
        }

        private void Show()
        {
            display = true;
            origin = mouse.current.pos;
            UpdateFound();
        }

        private void Hide()
        {
            query = "";
            selection = 0;
            display = false;
        }

        public void HandleMouse(Mouse mouse)
        {
            if (!display)
            {
                return;
            }

            MouseState current = mouse.current;
            var bounds = new Rect(origin.ToPoint(), new Size(WIDTH, HEIGHT));
            bool inBounds = bounds.Contains(current.pos.ToPoint());
            if (inBounds)
            {
                Vector2 relative = current.pos - origin;
                selection = (int)relative.Y / LEADING - 1;
            }
            else if (!inBounds && current.left)
            {
                Hide();
            }
            UpdateSeletion();
        }

        private void UpdateSeletion()
        {
            selection = Math.Min(selection, found.Count - 1);
            selection = found.Count > 0 && selection == -1 ? 0 : selection;
        }
    }
}
