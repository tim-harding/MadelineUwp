using Madeline.Backend;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.System;

namespace Madeline.Frontend
{
    internal class CreationDialogHandler : MouseHandler, KeypressHandler
    {
        public const int WIDTH = 150;
        public const int HEIGHT = 600;
        public const int LEADING = 25;

        public Mouse mouse;
        public Viewport viewport;
        public List<TableEntry<Plugin>> found = new List<TableEntry<Plugin>>();

        public string query = "";
        public int selection = 0;
        public int failPoint;
        public bool display;
        public Vector2 origin;

        public Vector2 Size => new Vector2(WIDTH, HEIGHT);
        public Vector2 Line => Vector2.UnitY * LEADING;

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
                viewport.graph.InsertNode(pos, found[selection].id);
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
            foreach (TableEntry<Plugin> plugin in viewport.graph.plugins)
            {
                bool alwaysMatch = query.Length == 0;
                bool matchQuery = plugin.value.name.Contains(query);
                if (alwaysMatch || matchQuery)
                {
                    found.Add(plugin);
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
