namespace Madeline.Frontend.Layout
{
    public struct Range
    {
        public int start;
        public int end;

        public int Length => end - start;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
