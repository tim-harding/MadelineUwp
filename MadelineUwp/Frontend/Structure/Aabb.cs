using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend.Structure
{
    internal struct Aabb
    {
        public Vector2 start;
        public Vector2 end;

        public Aabb(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public Rect ToRect()
        {
            return new Rect(start.ToPoint(), end.ToPoint());
        }

        public static Aabb Zero = new Aabb(Vector2.Zero, Vector2.Zero);
    }
}
