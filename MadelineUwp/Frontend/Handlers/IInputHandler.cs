using Windows.System;

namespace Madeline.Frontend.Handlers
{
    internal interface IInputHandler
    {
        bool HandleMouse();
        bool HandleKeypress(VirtualKey key);
        bool HandleScroll(int delta);
    }
}
