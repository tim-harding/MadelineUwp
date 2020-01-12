﻿using System.Collections.Generic;

namespace Madeline.Frontend
{
    internal class Components
    {
        public List<int> nodes = new List<int>();
        public List<Slot> wires = new List<Slot>();

        public void Clear()
        {
            nodes.Clear();
            wires.Clear();
        }
    }
}
