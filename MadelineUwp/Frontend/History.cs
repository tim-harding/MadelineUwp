using Madeline.Backend;
using System.Collections.Generic;

namespace Madeline.Frontend
{
    internal abstract class Action
    {
        public abstract void Redo(Graph graph);
        public abstract void Undo(Graph graph);
    }

    internal class History
    {
        private List<Action> queue;
        private int current = 0;
        private int head = 0;
        private int tail = 1;

        private Graph graph;

        public History(Graph graph)
        {
            this.graph = graph;

            const int MEMORY = 8;
            queue = new List<Action>(MEMORY);
            for (int i = 0; i < MEMORY; i++)
            {
                queue.Add(null);
            }
        }

        public void Redo()
        {
            if (current == Previous(tail)) { return; }

            Action action = queue[current];
            action.Redo(graph);
            current = Next(current);
        }

        public void Undo()
        {
            if (current == head) { return; }
            current = Previous(current);

            Action action = queue[current];
            action.Undo(graph);
        }

        public void SubmitChange(Action action)
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
