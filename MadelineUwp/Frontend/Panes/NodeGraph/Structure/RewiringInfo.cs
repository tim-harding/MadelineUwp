namespace Madeline.Frontend.Panes.NodeGraph.Structure
{
    internal class RewiringInfo
    {
        public Slot src = Slot.Empty;
        public Slot dst = Slot.Empty;

        public int upstream;
        public bool bidirectional;
    }
}
