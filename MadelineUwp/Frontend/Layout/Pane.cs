using Madeline.Frontend.Drawing;
using Madeline.Frontend.Handlers;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Windows.System;

namespace Madeline.Frontend.Layout
{
    internal class Pane : IInputHandler, IDrawer
    {
        public Rect rect;

        private IInputHandler[] handlers;
        private IDrawer[] drawers;

        private delegate bool ProcessInput(IInputHandler handler);

        public Pane(IInputHandler[] handlers, IDrawer[] drawers)
        {
            this.handlers = handlers;
            this.drawers = drawers;
        }

        public void Draw()
        {
            float width = (float)rect.Width;
            float height = (float)rect.Height;
            var target = new CanvasRenderTarget(Globals.session.Device, width, height, Globals.session.Dpi);
            using (CanvasDrawingSession offscreen = target.CreateDrawingSession())
            {
                CanvasDrawingSession tmp = Globals.session;
                Globals.session = offscreen;
                Globals.session.Clear(Palette.Tone3);
                foreach (IDrawer drawer in drawers)
                {
                    drawer.Draw();
                }
                Globals.session = tmp;
            }
            Globals.session.DrawImage(target, rect);
        }

        public bool HandleKeypress(VirtualKey key)
        {
            return Handle((IInputHandler handler) => handler.HandleKeypress(key));
        }

        public bool HandleMouse()
        {
            return Handle((IInputHandler handler) => handler.HandleMouse());
        }

        public bool HandleScroll(int delta)
        {
            return Handle((IInputHandler handler) => handler.HandleScroll(delta));
        }

        private bool Handle(ProcessInput callback)
        {
            foreach (IInputHandler handler in handlers)
            {
                if (callback(handler))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
