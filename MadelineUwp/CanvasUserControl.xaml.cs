using Madeline.Frontend;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Madeline
{
    public sealed partial class CanvasUserControl : UserControl
    {
        internal IInputHandler[] handlers;
        internal IDrawer[] drawers;

        private delegate bool ProcessInput(IInputHandler handler);

        public CanvasUserControl()
        {
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Globals.session = args.DrawingSession;
            foreach (IDrawer drawer in drawers)
            {
                drawer.Draw();
            }
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void HandleScroll(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(canvas).Properties.MouseWheelDelta;
            Handle((IInputHandler handler) => handler.HandleScroll(delta));
            canvas.Invalidate();
        }

        private void HandleMouse(object sender, PointerRoutedEventArgs e)
        {
            UpdateMouse(e);
            Handle((IInputHandler handler) => handler.HandleMouse());
            canvas.Invalidate();
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;
            Handle((IInputHandler handler) => handler.HandleKeypress(key));
            canvas.Invalidate();
        }

        private void UpdateMouse(PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(canvas);
            PointerPointProperties props = point.Properties;
            var update = new Mouse.State
            {
                left = props.IsLeftButtonPressed,
                right = props.IsRightButtonPressed,
                middle = props.IsMiddleButtonPressed,
                pos = point.Position.ToVector2(),
            };
            Mouse.Update(update);
        }

        private void Handle(ProcessInput callback)
        {
            foreach (IInputHandler handler in handlers)
            {
                if (callback(handler))
                {
                    return;
                }
            }
        }
    }
}
