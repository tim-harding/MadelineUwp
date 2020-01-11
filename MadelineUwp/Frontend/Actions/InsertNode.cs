using Madeline.Backend;
using System.Numerics;

namespace Madeline.Frontend.Actions
{
    internal class InsertNode : Action
    {
        private int id = -1;
        private int pluginId;
        private Vector2 pos;

        public InsertNode(Graph graph, int pluginId, Vector2 pos)
        {
            this.pluginId = pluginId;
            this.pos = pos;
        }

        public override void Redo(Graph graph)
        {
            if (!graph.plugins.TryGet(pluginId, out Plugin plugin)) { return; }

            var node = new Node(pos, plugin, pluginId);
            switch (id)
            {
                case -1:
                    id = graph.nodes.Insert(node);
                    graph.inputs.Extend(plugin.inputs, -1);
                    break;

                default:
                    graph.nodes.InsertWithId(id, node);
                    graph.inputs.ExtendWithId(id, plugin.inputs, -1);
                    break;
            }
        }

        public override void Undo(Graph graph)
        {
            graph.nodes.Delete(id);
            graph.inputs.Delete(id);
        }
    }
}
