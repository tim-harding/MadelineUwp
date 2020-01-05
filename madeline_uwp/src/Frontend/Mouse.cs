using System.Numerics;

namespace Madeline
{
    internal struct MouseState
    {
        public bool left;
        public bool middle;
        public bool right;
        public Vector2 pos;
    }

    internal struct Mouse
    {
        public MouseState current;
        public MouseState previous;
    }
}
