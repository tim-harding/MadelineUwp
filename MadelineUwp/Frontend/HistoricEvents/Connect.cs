﻿using Madeline.Backend;

namespace Madeline.Frontend.HistoricEvents
{
    internal class Connect : HistoricEvent
    {
        private int downstream;
        private int upstream;
        private int inputSlot;

        public Connect(int upstream, int downstream, int inputSlot)
        {
            this.upstream = upstream;
            this.downstream = downstream;
            this.inputSlot = inputSlot;
        }

        public override void Redo(NodeGraph graph)
        {
            SetSlot(graph, upstream);
        }

        public override void Undo(NodeGraph graph)
        {
            SetSlot(graph, -1);
        }

        private void SetSlot(NodeGraph graph, int i)
        {
            if (graph.nodes.TryGet(downstream, out Node node))
            {
                node.inputs[inputSlot] = i;
            }
        }
    }
}
