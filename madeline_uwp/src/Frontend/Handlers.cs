using Microsoft.Graphics.Canvas;
using Windows.System;

namespace Madeline
{
    internal interface Drawer
    {
        void Draw(CanvasDrawingSession session);
    }

    internal interface MouseHandler
    {
        bool HandleMouse();
    }

    internal interface KeypressHandler
    {
        bool HandleKeypress(VirtualKey key);
    }
}
