using Microsoft.Graphics.Canvas;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline
{
    internal class DragSelectDrawer : Drawer
    {
        private Viewport viewport;

        public DragSelectDrawer(Viewport viewport)
        {
            this.viewport = viewport;
        }

        public void Draw(CanvasDrawingSession session)
        {
            bool dragging = viewport.selectBoxStart != viewport.selectBoxEnd;
            if (!dragging)
            {
                return;
            }
            var start = viewport.selectBoxStart.ToPoint();
            var end = viewport.selectBoxEnd.ToPoint();
            var rect = new Rect(start, end);
            var color = Color.FromArgb(64, 255, 255, 255);
            session.FillRectangle(rect, color);
            session.DrawRectangle(rect, color);
        }
    }
}
