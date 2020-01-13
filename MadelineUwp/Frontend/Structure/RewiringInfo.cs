﻿namespace Madeline.Frontend.Structure
{
    internal class RewiringInfo
    {
        public Slot src = Slot.Empty;
        public Slot dst = Slot.Empty;

        public int upstreamReference;
        public bool bidirectional;
    }
}
