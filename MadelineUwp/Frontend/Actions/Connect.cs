using Madeline.Backend;

namespace Madeline.Frontend.Actions
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

        public override void Redo(Graph graph)
        {
            SetSlot(graph, upstream);
        }

        public override void Undo(Graph graph)
        {
            SetSlot(graph, -1);
        }

        private void SetSlot(Graph graph, int i)
        {
            if (graph.nodes.TryGetRowForId(downstream, out int row))
            {
                Node node = graph.nodes.GetAtRow(row);
                node.inputs[inputSlot] = i;
                graph.nodes.UpdateAtRow(row, node);
            }
        }
    }
}
