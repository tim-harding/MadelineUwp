using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal struct LineSegment
    {
        public Vector2 start;
        public Vector2 end;

        public LineSegment(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public float Distance(Vector2 pos)
        {
            Vector2 dir = end - start;
            float len = dir.Length();
            if (len > 0f)
            {
                dir /= len;
            }
            Vector2 local = pos - start;
            float t = Vector2.Dot(local, dir);
            Vector2 proj = dir * t;

            float dist = Vector2.Distance(proj, local);
            if (t < 0 || t > len)
            {
                dist = float.MaxValue;
                float distToStart = Vector2.Distance(pos, start);
                float distToEnd = Vector2.Distance(pos, end);
                dist = Math.Min(distToStart, distToEnd);
            }

            return dist;
        }
    }

    internal struct Wire
    {
        public Vector2 iPos;
        public Vector2 oPos;

        public Wire(Vector2 iPos, Vector2 oPos)
        {
            Vector2 slotEnd = Vector2.UnitY * Slot.DISPLAY_RADIUS;
            this.oPos = oPos + slotEnd;
            this.iPos = iPos - slotEnd;
        }

        private const float PI = (float)Math.PI;

        public float Theta(Vector2 to, Vector2 from, float r)
        {
            Vector2 c = from + CircleOffset(r);
            Vector2 d = to - c;
            float n = (float)Math.Atan2(d.Y, Math.Abs(d.X));
            float m = (float)Math.Acos(r / d.Length());
            float theta = PI - n - m;
            theta *= Sign;
            return theta;
        }

        public float BaseRadius(bool rounded)
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

        public float FlattenedRadius(float r)
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

        public float Rightward => Convert.ToInt32(iPos.X > oPos.X);

        public float Sign => Rightward * 2f - 1f;

        public Vector2 CircleOffset(float r)
        {
            return Vector2.UnitX * r * Sign;
        }

        public float Distance(Vector2 pos)
        {
            float r = BaseRadius(true);
            r = FlattenedRadius(r);
            Vector2 c1 = oPos + CircleOffset(r);
            Vector2 c2 = iPos - CircleOffset(r);

            float theta = Theta(iPos, (iPos + oPos) / 2f, r);
            LineSegment line = BetweenArcs(c1, c2, theta, r);
            float dist = line.Distance(pos);
            dist = Math.Min(dist, new LineSegment(iPos, line.end).Distance(pos));
            dist = Math.Min(dist, new LineSegment(oPos, line.start).Distance(pos));
            return dist;
        }

        public LineSegment BetweenArcs(Vector2 c1, Vector2 c2, float theta, float r)
        {
            float angle = -theta;
            var startRot = Matrix3x2.CreateRotation(angle, c1);
            var start = Vector2.Transform(oPos, startRot);

            var endRot = Matrix3x2.CreateRotation(angle, c2);
            var end = Vector2.Transform(iPos, endRot);

            return new LineSegment(start, end);
        }
    }
}
