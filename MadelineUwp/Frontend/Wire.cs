using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal struct Wire
    {
        public Vector2 iPos;
        public Vector2 oPos;

        private const float PI = (float)Math.PI;

        public Wire(Vector2 iPos, Vector2 oPos)
        {
            this.iPos = iPos;
            this.oPos = oPos;
        }

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
    }
}
