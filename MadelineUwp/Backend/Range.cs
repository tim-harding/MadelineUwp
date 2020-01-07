namespace Madeline.Backend
{
    internal struct Range
    {
        public int start;
        public int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int Count => end - start;
    }
}
