﻿using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend.Panes.NodeGraph.Drawing.Structure
{
    internal struct CommandList
    {
        public CanvasCommandList list;
        public CanvasDrawingSession session;

        public CommandList(ICanvasResourceCreator device)
        {
            list = new CanvasCommandList(device);
            session = list.CreateDrawingSession();
        }
    }
}
