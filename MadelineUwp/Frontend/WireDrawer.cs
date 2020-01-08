using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.UI;

namespace Madeline.Frontend
{
    internal class WireDrawer
    {
        public static void DrawWire(
            CanvasDrawingSession session,
            Vector2 iPos,
            Vector2 oPos,
            Color color,
            float zoom,
            bool closed)
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
            r = Math.Min(r, len / 4f);
            r *= zoom;

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

            float rightward = Convert.ToInt32(iPos.X > oPos.X);
            float sign = rightward * 2f - 1f;
            var circleOffset = new Vector2(r * sign, 0f);
            Vector2 c1 = oPos + circleOffset;
            Vector2 c2 = iPos - circleOffset;
            Vector2 cd = c2 - c1;
            float n = (float)Math.Atan2(cd.Y, Math.Abs(cd.X));
            float m = (float)Math.Acos(2f * r / cd.Length());
            float pi = (float)Math.PI;
            float theta = pi - n - m;
            theta *= sign;

            var path = new CanvasPathBuilder(session.Device);
            path.BeginFigure(oPos);

            float start = rightward * pi;
            path.AddArc(c1, r, r, start, -theta);
            start = pi - start;
            path.AddArc(c2, r, r, start - theta, theta);

            path.EndFigure(CanvasFigureLoop.Open);
            var geo = CanvasGeometry.CreatePath(path);
            session.DrawGeometry(geo, color, 2f);

            // Use closed
        }
    }
}
