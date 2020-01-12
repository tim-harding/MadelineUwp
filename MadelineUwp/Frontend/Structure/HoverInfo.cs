namespace Madeline.Frontend.Structure
{
    internal class HoverInfo
    {
        public NodeHover node;
        public SlotProximity slot;
        public Slot wire;

        public HoverInfo()
        {
            Clear();
        }

        public HoverInfo(HoverInfo src)
        {
            node = src.node;
            slot = src.slot;
            wire = src.wire;
        }

        public void Clear()
        {
            node = NodeHover.Empty;
            slot = SlotProximity.Empty;
            wire = Slot.Empty;
        }
    }
}
