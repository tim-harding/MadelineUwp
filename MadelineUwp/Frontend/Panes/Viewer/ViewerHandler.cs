using Windows.System;

namespace Madeline.Frontend.Panes.Viewer
{
    internal class ViewerHandler : IInputHandler
    {
        public bool HandleKeypress(VirtualKey key)
        {
            return false;
        }

        public bool HandleMouse()
        {
            return false;
        }

        public bool HandleScroll(int delta)
        {
            return false;
        }
    }
}
