using Madeline.Backend;

namespace Madeline.Frontend.HistoricEvents
{
    internal class DeleteNodes : HistoricEvent
    {
        private int[] ids;
        private Node[] nodes;

        public DeleteNodes(int[] ids, Node[] nodes)
        {
            this.ids = ids;
            this.nodes = nodes;
        }

        public override void Redo(Graph graph)
        {
            foreach (int id in ids)
            {
                graph.nodes.Delete(id);
            }
        }

        public override void Undo(Graph graph)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                Node node = nodes[i];
                graph.nodes.InsertWithId(id, node);
            }
        }
    }
}
