using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal struct MouseState
    {
        public bool left;
        public bool middle;
        public bool right;
        public Vector2 pos;
    }

    internal enum MouseButton
    {
        None,
        Down,
        Up,
        Dragging,
    }

    internal class Mouse
    {
        public MouseState current;
        public MouseState previous;

        public MouseButton Left => State(current.left, previous.left);

        public MouseButton Right => State(current.right, previous.right);

        public MouseButton Middle => State(current.middle, previous.middle);

        public Vector2 Delta => current.pos - previous.pos;

        private MouseButton State(bool current, bool previous)
        {
            int now = Convert.ToInt32(current);
            int then = Convert.ToInt32(previous);
            int sum = now + then * 2;
            return (MouseButton)sum;
        }
    }
}
