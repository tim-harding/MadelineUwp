using System;
using System.Numerics;
using Madeline.Backend;

namespace Madeline.Frontend
{
    internal class Viewport
    {
        public Graph graph = SampleData.DefaultGraph();
        public Vector2 transform = new Vector2(400, 300);
        public float zoom = 1f;

        public int viewing = -1;
        public GraphComponentsSingle hover = new GraphComponentsSingle();
        public GraphComponentsSingle active = new GraphComponentsSingle();
        public GraphComponentsMulti selectionCandidates = new GraphComponentsMulti();
        public GraphComponentsMulti selection = new GraphComponentsMulti();

        public Vector2 selectBoxStart;
        public Vector2 selectBoxEnd;

        public Slot wireSrc;
        public Slot wireDst;

        public void ZoomAround(Vector2 pos, int delta)
        {
            float factor = (float)Math.Pow(1.001, delta);
            zoom *= factor;
            transform = From(Into(transform) - pos * (factor - 1f));
        }

        public void Move(Vector2 delta)
        {
            transform += delta * 1f / zoom;
        }

        public Vector2 Into(Vector2 pos)
        {
            return (pos + transform) * zoom;
        }

        public Vector2 From(Vector2 pos)
        {
            return pos / zoom - transform;
        }
    }
}
