﻿using Madeline.Backend;
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
        private NodeCreationDialog dialog;
        private NodesDrawer nodesDrawer;
        private NodeInteraction nodeInteraction;
        private Hover hover;
        private Mouse mouse = new Mouse();

        public NodeGraph()
        {
            dialog = new NodeCreationDialog(viewport, mouse);
            nodesDrawer = new NodesDrawer(viewport);
            nodeInteraction = new NodeInteraction(mouse, viewport);
            hover = new Hover(mouse, viewport);
            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            nodesDrawer.Draw(session);
            dialog.drawer.Draw(session);
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
            hover.HandleMouse();
            bool _ = dialog.HandleMouse() || nodeInteraction.HandleMouse();
        }

        private void HandleKeypress(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;
            if (args.KeyStatus.WasKeyDown)
            {
                bool handled = dialog.HandleKeyboard(key) || nodeInteraction.HandleKeypress(key);
                if (handled)
                {
                    canvas.Invalidate();
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
