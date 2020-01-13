using Windows.System;

namespace Madeline.Frontend.Handlers
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
