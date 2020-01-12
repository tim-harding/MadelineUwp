using Microsoft.Graphics.Canvas;
using Windows.System;

namespace Madeline.Frontend
{
    internal interface IDrawer
    {
        void Draw(CanvasDrawingSession session);
    }

    internal interface IMouseHandler
    {
        bool HandleMouse();
    }

    internal interface IKeypressHandler
    {
        bool HandleKeypress(VirtualKey key);
    }
}
