namespace Madeline.Frontend.Panes.NodeGraph.Structure
{
    internal class SelectionInfo
    {
        public Components candidates = new Components();
        public Components active = new Components();
        public Aabb box;
    }
}
