using System.Numerics;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Madeline
{
    internal static class Utils
    {
        public static int Mod(int i, int m)
        {
            return (i % m + m) % m;
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }

        public static bool IsKeyDown(VirtualKey key)
        {
            CoreWindow window = Window.Current.CoreWindow;
            CoreVirtualKeyStates state = window.GetKeyState(key);
            return state.HasFlag(CoreVirtualKeyStates.Down);
        }

        public static Vector2 Origin(this Rect rect)
        {
            var point = new Point(rect.Left, rect.Top);
            return point.ToVector2();
        }

        public static Vector2 Size(this Rect rect)
        {
            var point = new Point(rect.Right - rect.Left, rect.Bottom - rect.Top);
            return point.ToVector2();
        }
    }
}
