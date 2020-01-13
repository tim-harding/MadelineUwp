using Windows.System;

namespace Madeline.Frontend
{
    internal interface IDrawer
    {
        void Draw();
    }

    internal interface IInputHandler
    {
        bool HandleMouse();
        bool HandleKeypress(VirtualKey key);
        bool HandleScroll(int delta);
    }
}
