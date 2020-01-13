using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class Mouse
    {
        public enum Button
        {
            None,
            Down,
            Up,
            Dragging,
        }

        internal struct State
        {
            public bool left;
            public bool middle;
            public bool right;
            public Vector2 pos;

            public bool AnyDown()
            {
                return left || right || middle;
            }
        }

        private static State current;
        private static State previous;

        public static Button Left => ButtonState(current.left, previous.left);
        public static Button Right => ButtonState(current.right, previous.right);
        public static Button Middle => ButtonState(current.middle, previous.middle);
        public static Vector2 Delta => current.pos - previous.pos;
        public static Vector2 Relative => current.pos - Globals.pane.rect.Origin();

        public static void Update(State update)
        {
            previous = current;
            current = update;
        }

        private Mouse() { }

        private static Button ButtonState(bool current, bool previous)
        {
            int now = Convert.ToInt32(current);
            int then = Convert.ToInt32(previous);
            int sum = now + then * 2;
            return (Button)sum;
        }
    }
}
