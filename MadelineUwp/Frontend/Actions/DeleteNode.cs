﻿using Madeline.Backend;

namespace Madeline.Frontend.Actions
{
    internal class DeleteNode : HistoricEvent
    {
        public DeleteNode() {  }

        public override void Redo(Graph graph)
        {
            // base.Undo(graph);
        }

        public override void Undo(Graph graph)
        {
            // base.Redo(graph);
        }
    }
}
