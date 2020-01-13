using Madeline.Frontend.Panes.NodeGraph.Structure;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;

namespace Madeline.Frontend.Panes.NodeGraph.Drawing
{
    internal struct Wire
    {
        internal enum Kind
        {
            DoubleEnded,
            Down,
            Up,
        }

        private const float PI = (float)Math.PI;

        public Vector2 iPos;
        public Vector2 oPos;
        public Kind kind;

        public float Rightward => Convert.ToInt32(iPos.X > oPos.X);

        public float Sign => Rightward * 2f - 1f;

        public Wire(Vector2 iPos, Vector2 oPos, Kind kind)
        {
            Vector2 slotEnd = Vector2.UnitY * Slot.DISPLAY_RADIUS;
            this.oPos = oPos + slotEnd;
            this.iPos = iPos - slotEnd;
            this.kind = kind;
        }

        public Vector2 CircleOffset(float r)
        {
            return Vector2.UnitX * r * Sign;
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

        public CanvasGeometry Geo(CanvasDrawingSession session)
        {
            bool rounded = kind == Kind.DoubleEnded;
            float r = BaseRadius(rounded);
            r = rounded ? FlattenedRadius(r) : r;
            Vector2 target = rounded ? (iPos + oPos) / 2f : oPos;
            float theta = Theta(iPos, target, r);

            var path = new CanvasPathBuilder(session.Device);
            path.BeginFigure(oPos);

            switch (kind)
            {
                case Kind.DoubleEnded:
                    UpperArc(path, r, theta);
                    LowerArc(path, r, theta);
                    break;

                case Kind.Down:
                    UpperArc(path, r, theta);
                    path.AddLine(iPos + Vector2.UnitY * Slot.DISPLAY_RADIUS);
                    break;

                case Kind.Up:
                    path.AddLine(oPos - Vector2.UnitY * Slot.DISPLAY_RADIUS);
                    LowerArc(path, r, theta);
                    break;
            }

            path.EndFigure(CanvasFigureLoop.Open);
            var geo = CanvasGeometry.CreatePath(path);
            return geo;
        }

        private void UpperArc(CanvasPathBuilder path, float r, float theta)
        {
            float start = Rightward * PI;
            Vector2 c1 = oPos + CircleOffset(r);
            path.AddArc(c1, r, r, start, -theta);
        }

        private void LowerArc(CanvasPathBuilder path, float r, float theta)
        {
            float start = (1f - Rightward) * PI;
            Vector2 c2 = iPos - CircleOffset(r);
            path.AddArc(c2, r, r, start - theta, theta);
        }
    }
}
