using Madeline.Backend;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend.Drawers
{
    internal struct BodyGeo
    {
        public CanvasGeometry disable;
        public CanvasGeometry clipper;
        public CanvasGeometry viewing;

        public BodyGeo(ICanvasResourceCreator device)
        {
            var rect = new Rect(Vector2.Zero.ToPoint(), Node.Size.ToSize());
            const float ROUNDING = 5f;
            clipper = CanvasGeometry.CreateRoundedRectangle(device, rect, ROUNDING, ROUNDING);

            var size = new Vector2(20f, 60f);
            rect = new Rect((-size).ToPoint(), size.ToPoint());
            var verticalCenter = Matrix3x2.CreateTranslation(0f, Node.Size.Y / 2f);
            var rotate = Matrix3x2.CreateRotation(0.2f);
            disable = CanvasGeometry.CreateRectangle(device, rect);
            disable = disable.Transform(rotate * verticalCenter);
            var farSideTx = Matrix3x2.CreateTranslation(Node.Size.X, 0f);
            viewing = disable.Transform(farSideTx);
        }

        public BodyGeo Transform(Matrix3x2 tx)
        {
            return new BodyGeo()
            {
                clipper = clipper.Transform(tx),
                disable = disable.Transform(tx),
                viewing = viewing.Transform(tx),
            };
        }
    }
}
