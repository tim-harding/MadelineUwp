using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class GraphComponentsSingle
    {
        public int node;
        public Slot slot;
        public Slot wire;

        public GraphComponentsSingle()
        {
            Clear();
        }

        public void Clear()
        {
            node = -1;
            slot = new Slot(-1, -1);
            wire = new Slot(-1, -1);
        }
    }

    internal class GraphComponentsMulti
    {
        public List<int> nodes = new List<int>();
        public List<Slot> slots = new List<Slot>();
        public List<Slot> wires = new List<Slot>();

        public void Clear()
        {
            nodes.Clear();
            slots.Clear();
            wires.Clear();
        }
    }
}
