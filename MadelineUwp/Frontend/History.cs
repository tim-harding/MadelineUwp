using Madeline.Backend;
using System.Collections.Generic;

namespace Madeline.Frontend
{
    internal abstract class HistoricEvent
    {
        public abstract void Redo(NodeGraph graph);
        public abstract void Undo(NodeGraph graph);
    }

    internal class History
    {
        private List<HistoricEvent> queue;
        private int current = 0;
        private int head = 0;
        private int tail = 1;

        private NodeGraph graph;

        public History(NodeGraph graph)
        {
            this.graph = graph;

            const int MEMORY = 8;
            queue = new List<HistoricEvent>(MEMORY);
            for (int i = 0; i < MEMORY; i++)
            {
                queue.Add(null);
            }
        }

        public void Redo()
        {
            if (current == Previous(tail)) { return; }

            HistoricEvent action = queue[current];
            action.Redo(graph);
            current = Next(current);
        }

        public void Undo()
        {
            if (current == head) { return; }
            current = Previous(current);

            HistoricEvent action = queue[current];
            action.Undo(graph);
        }

        public void SubmitChange(HistoricEvent action)
        {
            action.Redo(graph);
            queue[current] = action;
            current = Next(current);
            tail = Next(current);
            if (head == current)
            {
                head = tail;
            }
        }

        private int Next(int value)
        {
            return Mod(value + 1, queue.Count);
        }

        private int Previous(int value)
        {
            return Mod(value - 1, queue.Count);
        }

        private int Mod(int i, int m)
        {
            return (i % m + m) % m;
        }
    }
}
