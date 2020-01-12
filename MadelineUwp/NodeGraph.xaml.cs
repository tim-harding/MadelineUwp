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

namespace Madeline
{
    public sealed partial class NodeGraph : Page
    {
        private Viewport viewport = new Viewport();
        private Mouse mouse = new Mouse();

        private Frontend.Drawing.IDrawer[] drawers;
        private IMouseHandler[] mouseHandlers;
        private IKeypressHandler[] keypressHandlers;

        public NodeGraph()
        {
            var dialog = new Frontend.Handlers.CreationDialog(viewport, mouse);
            var nodes = new Frontend.Handlers.Nodes(viewport, mouse);
            var dragSelect = new Frontend.Handlers.DragSelect(viewport, mouse);
            var wireCreation = new Frontend.Handlers.WireCreation(viewport, mouse);

            mouseHandlers = new IMouseHandler[]
            {
                nodes,
                wireCreation,
                dragSelect,
            };

            keypressHandlers = new IKeypressHandler[]
            {
                dialog,
                nodes,
            };

            drawers = new Frontend.Drawing.IDrawer[]
            {
                new Frontend.Drawing.Nodes.Drawing(viewport, mouse),
                new Frontend.Drawing.WireCreation(viewport, mouse),
                new Frontend.Drawing.CreationDialog(dialog),
            };

            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;

            SampleData.Init(viewport);

            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            foreach (Frontend.Drawing.IDrawer drawer in drawers)
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
