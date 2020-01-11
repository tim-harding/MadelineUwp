using Madeline.Backend;

namespace Madeline.Frontend.Actions
{
    internal class Connect : Action
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
            graph.inputs.Update(downstream, inputSlot, upstream);
        }

        public override void Undo(Graph graph)
        {
            graph.inputs.Update(downstream, inputSlot, -1);
        }
    }
}
