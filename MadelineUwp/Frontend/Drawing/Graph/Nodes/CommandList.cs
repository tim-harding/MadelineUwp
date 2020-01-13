using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend.Drawing.Graph.Nodes
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
