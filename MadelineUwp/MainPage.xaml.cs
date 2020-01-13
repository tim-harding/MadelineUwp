using Madeline.Frontend;
using Madeline.Frontend.Layout;
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
    public sealed partial class MainPage : Page
    {
        private PaneGrid grid = new PaneGrid
        (
            new Frontend.Layout.Grid(
                new float[] { 1f, 1f },
                new float[] { 3f, 1f }
            ),
            new Occupant[]
            {
                new Occupant(
                    new Range(1, 1),
                    new Range(0, 0),
                    Panes.NodeGraphPane()
                ),
            }
        );

        public MainPage()
        {
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            Globals.Init();
            SampleData.Init();
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Globals.session = args.DrawingSession;
            grid.Draw();
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void HandleScroll(object sender, PointerRoutedEventArgs e)
        {
            int delta = e.GetCurrentPoint(canvas).Properties.MouseWheelDelta;
            grid.HandleScroll(delta);
            canvas.Invalidate();
        }

        private void HandleMouse(object sender, PointerRoutedEventArgs e)
        {
            UpdateMouse(e);
            grid.HandleMouse();
            canvas.Invalidate();
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;
            grid.HandleKeypress(key);
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
    }
}
