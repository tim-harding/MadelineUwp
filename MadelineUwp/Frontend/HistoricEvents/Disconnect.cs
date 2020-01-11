using Madeline.Backend;

namespace Madeline.Frontend.HistoricEvents
{
    internal class Disconnect : Connect
    {
        public Disconnect(int node, int output, int input) : base(node, output, input) { }

        public override void Redo(Graph graph)
        {
            base.Undo(graph);
        }

        public override void Undo(Graph graph)
        {
            base.Redo(graph);
        }
    }
}
