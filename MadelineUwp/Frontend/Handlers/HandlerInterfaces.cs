using Windows.System;

namespace Madeline.Frontend
{
    internal interface IMouseHandler
    {
        bool HandleMouse();
    }

    internal interface IKeypressHandler
    {
        bool HandleKeypress(VirtualKey key);
    }
}
