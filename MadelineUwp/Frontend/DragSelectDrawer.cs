using Microsoft.Graphics.Canvas;
using Windows.UI;

namespace Madeline.Frontend
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
            SelectionInfo select = viewport.selection;
            bool dragging = select.box.start != select.box.end;
            if (!dragging) { return; }

            var rect = select.box.ToRect();
            var color = Color.FromArgb(64, 255, 255, 255);
            session.FillRectangle(rect, color);
            session.DrawRectangle(rect, color);
        }
    }
}
