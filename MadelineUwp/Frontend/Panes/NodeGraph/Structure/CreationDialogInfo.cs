using System.Collections.Generic;
using System.Numerics;

namespace Madeline.Frontend.Panes.NodeGraph.Structure
{
    internal class CreationDialogInfo
    {
        public List<string> found = new List<string>();
        public string query = "";
        public int selection = 0;
        public int failPoint;
        public bool display;
        public Vector2 origin;
    }
}
