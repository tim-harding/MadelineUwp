using Madeline.Frontend.Panes.NodeGraph.Structure;
using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend.Panes.NodeGraph.Drawing.Structure
{
    internal class Context
    {
        public CommandList wires;
        public CommandList nodes;
        public CommandList texts;

        public BaseGeo geo;
        public CanvasDrawingSession session;

        public Context(CanvasDrawingSession session, Viewport viewport)
        {
            CanvasDevice device = session.Device;
            wires = new CommandList(device);
            nodes = new CommandList(device);
            texts = new CommandList(device);
            geo = new BaseGeo(device, viewport);
            this.session = session;
        }
    }
}
