using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.System;

namespace Madeline.Frontend.Handlers.Graph
{
    internal class CreationDialogHandler : IMouseHandler, IKeypressHandler
    {
        public const float WIDTH = 120f;
        public const float MARGIN = 10f;
        public const float LINE_HEIGHT = 38f;

        public Mouse mouse;
        public Viewport viewport;
        public List<string> found = new List<string>();

        public string query = "";
        public int selection = 0;
        public int failPoint;
        public bool display;
        public Vector2 origin;

        public Vector2 Line => Vector2.UnitY * LINE_HEIGHT;

        public CreationDialogHandler(Viewport viewport, Mouse mouse)
        {
            this.viewport = viewport;
            this.mouse = mouse;
        }

        public bool HandleKeypress(VirtualKey key)
        {
            if (key == VirtualKey.Tab)
            {
                Toggle();
                return true;
            }
            if (display)
            {
                HandleKey(key);
                return true;
            }
            return false;
        }

        public bool HandleMouse()
        {
            if (!display)
            {
                return false;
            }
            MouseState current = mouse.current;
            int lines = found.Count + 1;
            var size = new Vector2(WIDTH, LINE_HEIGHT * lines);
            var bounds = new Rect(origin.ToPoint(), size.ToSize());
            bool inBounds = bounds.Contains(current.pos.ToPoint());
            if (inBounds)
            {
                Vector2 relative = current.pos - origin;
                selection = (int)(relative.Y / LINE_HEIGHT) - 1;
            }
            else if (!inBounds && current.left)
            {
                Hide();
            }
            UpdateSeletion();
            return true;
        }

        private void HandleKey(VirtualKey key)
        {
            switch (key)
            {
                case VirtualKey.Escape:
                    Hide();
                    break;

                case VirtualKey.Back:
                    Backspace();
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
                    Commit();
                    break;

                default:
                    ConsumeInput(key);
                    break;
            }
        }

        private void Toggle()
        {
            if (display)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        private void Backspace()
        {
            if (query.Length > 0)
            {
                query = query.Remove(query.Length - 1);
                UpdateFound();
            }
        }

        private void Commit()
        {
            if (selection > -1 && selection < found.Count)
            {
                Vector2 pos = viewport.From(origin);
                string key = found[selection];
                Plugin plugin = viewport.graph.plugins[key];
                viewport.history.SubmitChange(new HistoricEvents.InsertNode(viewport.graph, plugin, pos));
            }
            Hide();
        }

        private void ConsumeInput(VirtualKey key)
        {
            char ascii = (char)key;
            bool space = ascii == 32;
            bool upper = ascii > 64 && ascii < 91;
            bool lower = ascii > 96 && ascii < 123;
            bool wanted = space || upper || lower;
            if (!wanted)
            {
                return;
            }
            query += char.ToLower(ascii);
            UpdateFound();
        }

        private void UpdateFound()
        {
            int previousCount = found.Count;
            foreach (string name in viewport.graph.plugins.Keys)
            {
                bool alwaysMatch = query.Length == 0;
                bool matchQuery = name.Contains(query);
                if (alwaysMatch || matchQuery)
                {
                    found.Add(name);
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

        private void UpdateSeletion()
        {
            selection = Math.Min(selection, found.Count - 1);
            selection = found.Count > 0 && selection == -1 ? 0 : selection;
        }
    }
}
