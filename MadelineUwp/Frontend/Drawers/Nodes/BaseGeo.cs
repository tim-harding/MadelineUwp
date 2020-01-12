using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;

namespace Madeline.Frontend.Drawers.Nodes
{
    internal struct BaseGeo
    {
        public BodyGeo body;
        public CanvasGeometry slot;
        public CanvasGeometry selectBox;

        public BaseGeo(ICanvasResourceCreator device, Viewport viewport)
        {
            body = new BodyGeo(device);
            slot = CanvasGeometry.CreateCircle(device, Vector2.Zero, Slot.DISPLAY_RADIUS);
            selectBox = CanvasGeometry.CreateRectangle(device, viewport.selection.box.ToRect());
        }
    }
}
