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
                Rect rect = grid.Occupant(occupant);
                pane.rect = rect;
                pane.Draw();
            }
        }

        public bool HandleKeypress(VirtualKey key)
        {
            foreach (Occupant occupant in occupants)
            {
                if (occupant.pane.HandleKeypress(key))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HandleMouse()
        {
            foreach (Occupant occupant in occupants)
            {
                if (occupant.pane.HandleMouse())
                {
                    return true;
                }
            }
            return false;
        }

        public bool HandleScroll(int delta)
        {
            throw new System.NotImplementedException();
        }
    }
}
