using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.UI;

namespace Madeline.Frontend
{
    internal enum WireKind
    {
        DoubleEnded,
        Down,
        Up,
    }

    internal class WireDrawer
    {
        private const float PI = (float)Math.PI;

        public static void DrawWire(
            CanvasDrawingSession session,
            Wire wire,
            Color color,
            WireKind kind)
        {
            bool rounded = kind == WireKind.DoubleEnded;
            float r = wire.BaseRadius(rounded);
            r = rounded ? wire.FlattenedRadius(r) : r;
            Vector2 target = rounded ? (wire.iPos + wire.oPos) / 2f : wire.oPos;
            float theta = wire.Theta(wire.iPos, target, r);

            var path = new CanvasPathBuilder(session.Device);
            path.BeginFigure(wire.oPos);

            switch (kind)
            {
                case WireKind.DoubleEnded:
                    UpperArc(path, wire, r, theta);
                    LowerArc(path, wire, r, theta);
                    break;

                case WireKind.Down:
                    UpperArc(path, wire, r, theta);
                    path.AddLine(wire.iPos);
                    break;

                case WireKind.Up:
                    path.AddLine(wire.oPos);
                    LowerArc(path, wire, r, theta);
                    break;
            }

            path.EndFigure(CanvasFigureLoop.Open);
            var geo = CanvasGeometry.CreatePath(path);
            session.DrawGeometry(geo, color, 2f);
        }

        private static void UpperArc(CanvasPathBuilder path, Wire wire, float r, float theta)
        {
            float start = wire.Rightward * PI;
            Vector2 c1 = wire.oPos + wire.CircleOffset(r);
            path.AddArc(c1, r, r, start, -theta);
        }

        private static void LowerArc(CanvasPathBuilder path, Wire wire, float r, float theta)
        {
            float start = (1f - wire.Rightward) * PI;
            Vector2 c2 = wire.iPos - wire.CircleOffset(r);
            path.AddArc(c2, r, r, start - theta, theta);
        }
    }
}
