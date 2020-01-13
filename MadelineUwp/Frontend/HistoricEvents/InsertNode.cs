using Madeline.Backend;
using System.Numerics;

namespace Madeline.Frontend.HistoricEvents
{
    internal class InsertNode : HistoricEvent
    {
        private int id = -1;
        private Plugin plugin;
        private Vector2 pos;

        public InsertNode(NodeGraph graph, Plugin plugin, Vector2 pos)
        {
            this.plugin = plugin;
            this.pos = pos;
        }

        public override void Redo(NodeGraph graph)
        {
            var node = new Node(pos, plugin);
            switch (id)
            {
                case -1:
                    id = graph.nodes.Insert(node);
                    break;

                default:
                    graph.nodes.InsertWithId(id, node);
                    break;
            }
        }

        public override void Undo(NodeGraph graph)
        {
            graph.nodes.Delete(id);
        }
    }
}
