﻿using Madeline.Frontend;
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

        private CreationDialogHandler dialog;
        private NodesHandler nodeInteraction;
        private DragSelectHandler dragSelect;
        private Hover hover;

        private Drawer[] drawers;
        private MouseHandler[] mouseHandlers;
        private KeypressHandler[] keypressHandlers;

        public NodeGraph()
        {
            hover = new Hover(mouse, viewport);
            dialog = new CreationDialogHandler(viewport, mouse);
            nodeInteraction = new NodesHandler(mouse, viewport);
            dragSelect = new DragSelectHandler(mouse, viewport);

            mouseHandlers = new MouseHandler[]
            {
                hover,
                dialog,
                nodeInteraction,
                dragSelect,
            };

            keypressHandlers = new KeypressHandler[]
            {
                dialog,
                nodeInteraction,
            };

            drawers = new Drawer[]
            {
                new CreationDialogDrawer(dialog),
                new NodesDrawer(viewport),
                new DragSelectDrawer(viewport),
            };

            Window.Current.CoreWindow.KeyDown += HandleKeypress;
            Window.Current.CoreWindow.KeyUp += HandleKeypress;
            InitializeComponent();
        }

        private void Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            CanvasDrawingSession session = args.DrawingSession;
            foreach (Drawer drawer in drawers)
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
            foreach (MouseHandler handler in mouseHandlers)
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
                foreach (KeypressHandler handler in keypressHandlers)
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