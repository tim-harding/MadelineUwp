namespace Madeline.Frontend
{
    internal struct Slot
    {
        // -1 is output
        // 0.. is input
        public int slot;
        public int node;

        public Slot(int node, int slot)
        {
            this.node = node;
            this.slot = slot;
        }

        public bool Match(int node, int slot)
        {
            return this.node == node && this.slot == slot;
        }
    }
}
