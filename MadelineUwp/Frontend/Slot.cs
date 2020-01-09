namespace Madeline.Frontend
{
    internal struct Slot
    {
        public const float DISPLAY_RADIUS = 4.5f;

        // -1 is output
        // 0.. is input
        public int slot;
        public int node;

        public static Slot Empty => new Slot(-1, -1);

        public Slot(int node, int slot)
        {
            this.node = node;
            this.slot = slot;
        }
    }
}
