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
    }
}
