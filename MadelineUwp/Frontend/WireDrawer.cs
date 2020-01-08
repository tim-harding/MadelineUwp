using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.UI;

namespace Madeline.Frontend
{
    internal class WireDrawer
    {
        private const float PI = (float)Math.PI;

        private static Vector2 iPos;
        private static Vector2 oPos;

        public static void DrawWire(
            CanvasDrawingSession session,
            Vector2 iPos,
            Vector2 oPos,
            Color color,
            float zoom,
            bool rounded)
        {
            WireDrawer.iPos = iPos;
            WireDrawer.oPos = oPos;

            float r = BaseRadius(rounded) * zoom;
            r = rounded ? FlattenedRadius(r) : r;
            Vector2 target = rounded ? (iPos + oPos) / 2f : oPos;
            float theta = Theta(iPos, target, r);

            var path = new CanvasPathBuilder(session.Device);
            path.BeginFigure(oPos);

            float start = Rightward * PI;
            Vector2 circleOffset = CircleOffset(r);
            Vector2 c1 = oPos + circleOffset;
            path.AddArc(c1, r, r, start, -theta);
            if (rounded)
            {
                start = PI - start;
                Vector2 c2 = iPos - circleOffset;
                path.AddArc(c2, r, r, start - theta, theta);
            }
            else
            {
                path.AddLine(iPos);
            }

            path.EndFigure(CanvasFigureLoop.Open);
            var geo = CanvasGeometry.CreatePath(path);
            session.DrawGeometry(geo, color, 2f);
        }

        private static float Theta(Vector2 to, Vector2 from, float r)
        {
            Vector2 c = from + CircleOffset(r);
            Vector2 d = to - c;
            float n = (float)Math.Atan2(d.Y, Math.Abs(d.X));
            float m = (float)Math.Acos(r / d.Length());
            float theta = PI - n - m;
            theta *= Sign;
            return theta;
        }

        private static float BaseRadius(bool rounded)
        {
            float r = 25f;
            Vector2 dir = iPos - oPos;
            float len = dir.Length();
            float mul = -(dir / len).Y * 0.5f + 0.5f;
            mul = 1f - mul;
            mul *= mul;
            mul *= mul;
            mul *= mul;
            mul = 1f - mul;
            r *= mul;
            len /= rounded ? 4f : 2f;
            r = Math.Min(r, len);
            return r;
        }

        private static float FlattenedRadius(float r)
        {
            float dist = iPos.Y - oPos.Y;
            if (dist < r * 2f / 3f)
            {
                float remap = Math.Clamp((-dist - 20f) / 40f, 0f, 1f);
                float rcp = 1f - remap;
                remap = rcp * remap * remap + remap * (1f - rcp * rcp);
                remap = remap * 2f / 3f + 1f / 3f;
                r *= remap;
            }
            else if (dist < 2f * r)
            {
                r = (iPos.Y - oPos.Y) / 2;
            }
            return r;
        }

        private static float Rightward => Convert.ToInt32(iPos.X > oPos.X);

        private static float Sign => Rightward * 2f - 1f;

        private static Vector2 CircleOffset(float r)
        {
            return Vector2.UnitX * r * Sign;
        }
    }
}
