using Madeline.Backend;
using Madeline.Frontend.Structure;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.System;

namespace Madeline.Frontend.Handlers.Graph
{
    internal class CreationDialogHandler : IInputHandler
    {
        public const float WIDTH = 120f;
        public const float MARGIN = 10f;
        public const float LINE_HEIGHT = 38f;

        private CreationDialogInfo info;
        private Viewport viewport;

        public Vector2 Line => Vector2.UnitY * LINE_HEIGHT;

        public CreationDialogHandler(Viewport viewport)
        {
            info = viewport.creationDialog;
            this.viewport = viewport;
        }

        public bool HandleScroll(int delta) { return false; }

        public bool HandleKeypress(VirtualKey key)
        {
            if (key == VirtualKey.Tab)
            {
                Toggle();
                return true;
            }
            if (info.display)
            {
                HandleKey(key);
                return true;
            }
            return false;
        }

        public bool HandleMouse()
        {
            if (!info.display) { return false; }

            int lines = info.found.Count + 1;
            var size = new Vector2(WIDTH, LINE_HEIGHT * lines);
            var bounds = new Rect(info.origin.ToPoint(), size.ToSize());
            bool inBounds = bounds.Contains(Mouse.Relative.ToPoint());
            bool down = Mouse.Left == Mouse.Button.Down;
            if (inBounds)
            {
                Vector2 relative = Mouse.Relative - info.origin;
                info.selection = (int)(relative.Y / LINE_HEIGHT) - 1;
            }
            else if (!inBounds && down)
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
                    info.selection += 1;
                    UpdateSeletion();
                    break;

                case VirtualKey.Up:
                    info.selection -= 1;
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
            if (info.display)
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
            if (info.query.Length > 0)
            {
                info.query = info.query.Remove(info.query.Length - 1);
                UpdateFound();
            }
        }

        private void Commit()
        {
            if (info.selection > -1 && info.selection < info.found.Count)
            {
                Vector2 pos = viewport.From(info.origin);
                string key = info.found[info.selection];
                Plugin plugin = Globals.graph.plugins[key];
                Globals.history.SubmitChange(new HistoricEvents.InsertNode(Globals.graph, plugin, pos));
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
            info.query += char.ToLower(ascii);
            UpdateFound();
        }

        private void UpdateFound()
        {
            int previousCount = info.found.Count;
            foreach (string name in Globals.graph.plugins.Keys)
            {
                bool alwaysMatch = info.query.Length == 0;
                bool matchQuery = name.Contains(info.query);
                if (alwaysMatch || matchQuery)
                {
                    info.found.Add(name);
                }
            }

            bool searchFailure = info.found.Count == previousCount;
            if (searchFailure)
            {
                info.failPoint = info.failPoint > -1 ? info.failPoint : info.query.Length - 1;
            }
            else
            {
                info.found.RemoveRange(0, previousCount);
                info.failPoint = -1;
            }

            UpdateSeletion();
        }

        private void Show()
        {
            info.display = true;
            info.origin = Mouse.Relative;
            UpdateFound();
        }

        private void Hide()
        {
            info.query = "";
            info.selection = 0;
            info.display = false;
        }

        private void UpdateSeletion()
        {
            info.selection = Math.Min(info.selection, info.found.Count - 1);
            info.selection = info.found.Count > 0 && info.selection == -1 ? 0 : info.selection;
        }
    }
}
