namespace Madeline.Frontend.Panes.NodeGraph.Structure
{
    internal struct NodeHover
    {
        public enum State
        {
            Body,
            Disable,
            Viewing,
        }

        public int id;
        public State state;

        public NodeHover(int id, State state)
        {
            this.id = id;
            this.state = state;
        }

        public static NodeHover Empty = new NodeHover(-1, State.Body);
    }
}
