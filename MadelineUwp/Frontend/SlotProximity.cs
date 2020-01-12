namespace Madeline.Frontend
{
    internal struct SlotProximity
    {
        public float distance;
        public Slot slot;

        public bool IsHover => distance < 256f;

        public SlotProximity(float distance, Slot slot)
        {
            this.distance = distance;
            this.slot = slot;
        }

        public static SlotProximity Empty = new SlotProximity(float.MaxValue, Slot.Empty);
    }
}
