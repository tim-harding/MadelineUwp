using Madeline.Frontend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Madeline.Frontend.Handlers;
using Madeline.Frontend.Drawing;
using Madeline.Frontend.Handlers.Graph;

namespace Madeline
{
    public sealed partial class MainPage : Page
    {
        private Viewport viewport = new Viewport();
        private Mouse mouse = new Mouse();

        private IDrawer[] drawers;
        private IMouseHandler[] mouseHandlers;
        private IKeypressHandler[] keypressHandlers;

        public MainPage()
        {
            var dialog = new CreationDialogHandler(viewport, mouse);
            var nodes = new NodesHandler(viewport, mouse);
            var dragSelect = new DragSelectHandler(viewport, mouse);
            var wireCreation = new WireCreationHandler(viewport, mouse);

            mouseHandlers = new IMouseHandler[]
            {
                dialog,
                nodes,
                wireCreation,
                dragSelect,
            };

            keypressHandlers = new IKeypressHandler[]
            {
                dialog,
                nodes,
            };

            drawers = new IDrawer[]
            {
                new NodesDrawer(viewport, mouse),
                new WireCreationDrawer(viewport, mouse),
                new CreationDialogDrawer(dialog),
            };

            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;

            SampleData.Init(viewport);

            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            foreach (IDrawer drawer in drawers)
            {
                drawer.Draw(session);
            }
        }

        private void Unload(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        private void HandleScroll(object sender, PointerRoutedEventArgs e)
        {
            int wheel = e.GetCurrentPoint(canvas).Properties.MouseWheelDelta;
            viewport.ZoomAround(mouse.current.pos, wheel);
            canvas.Invalidate();
        }

        private void HandleMouse(object sender, PointerRoutedEventArgs e)
        {
            UpdateMouse(e);
            canvas.Invalidate();
            foreach (IMouseHandler handler in mouseHandlers)
            {
                if (handler.HandleMouse())
                {
                    break;
                }
            }
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;
            canvas.Invalidate();
            if (args.KeyStatus.WasKeyDown)
            {
                foreach (IKeypressHandler handler in keypressHandlers)
                {
                    if (handler.HandleKeypress(key))
                    {
                        break;
                    }
                }
            }
        }

        private void UpdateMouse(PointerRoutedEventArgs e)
        {
            PointerPoint point = e.GetCurrentPoint(canvas);
            PointerPointProperties props = point.Properties;
            mouse.previous = mouse.current;
            mouse.current = new MouseState
            {
                left = props.IsLeftButtonPressed,
                right = props.IsRightButtonPressed,
                middle = props.IsMiddleButtonPressed,
                pos = point.Position.ToVector2(),
            };
        }
    }
}
