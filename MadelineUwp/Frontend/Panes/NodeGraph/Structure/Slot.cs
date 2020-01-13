namespace Madeline.Frontend.Panes.NodeGraph.Structure
{
    internal struct Slot
    {
        public const float DISPLAY_RADIUS = 4.5f;

        // -1 is output
        // 0.. is input
        public int index;
        public int node;

        public static Slot Empty => new Slot(-1, -1);

        public Slot(int node, int index)
        {
            this.node = node;
            this.index = index;
        }
    }
}
