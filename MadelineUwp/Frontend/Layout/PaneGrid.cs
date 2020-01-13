using Madeline.Frontend.Drawing;
using Madeline.Frontend.Handlers;
using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Xaml;

namespace Madeline.Frontend.Layout
{
    internal class PaneGrid : IInputHandler, IDrawer
    {
        private Grid grid;
        private Occupant[] occupants;

        private delegate bool ProcessInput(IInputHandler handler);

        public PaneGrid(Grid grid, Occupant[] occupants)
        {
            this.grid = grid;
            this.occupants = occupants;
        }

        public void Draw()
        {
            Rect bounds = Window.Current.Bounds;
            var size = new Size(bounds.Width, bounds.Height);
            grid.dimensions = size.ToVector2();
            foreach (Occupant occupant in occupants)
            {
                Pane pane = occupant.pane;
                Globals.pane = pane;
                Rect rect = grid.Occupant(occupant);
                pane.rect = rect;
                pane.Draw();
            }
        }

        public bool HandleKeypress(VirtualKey key)
        {
            return Handle((IInputHandler handler) => handler.HandleKeypress(key));
        }

        public bool HandleMouse()
        {
            return Handle((IInputHandler handler) => handler.HandleMouse());
        }

        public bool HandleScroll(int delta)
        {
            return Handle((IInputHandler handler) => handler.HandleScroll(delta));
        }

        private bool Handle(ProcessInput callback)
        {
            foreach (Occupant occupant in occupants)
            {
                if (callback(occupant.pane))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
