using Microsoft.Graphics.Canvas;

namespace Madeline.Frontend.Drawers
{
    internal struct Context
    {
        public CommandList wires;
        public CommandList nodes;
        public CommandList texts;

        public BaseGeo geo;

        public Context(ICanvasResourceCreator device, Viewport viewport)
        {
            wires = new CommandList(device);
            nodes = new CommandList(device);
            texts = new CommandList(device);
            geo = new BaseGeo(device, viewport);
        }
    }
}
