using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend.Drawing
{
    internal interface IDrawer
    {
        void Draw(CanvasDrawingSession session);
    }
}
