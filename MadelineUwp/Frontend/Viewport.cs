using Madeline.Frontend.Structure;
using System;
using System.Numerics;

namespace Madeline.Frontend
{
    internal class Viewport
    {
        public Vector2 translate;
        public float zoom = 1f;
        public int viewing = -1;
        public int active = -1;

        public HoverInfo hover = new HoverInfo();
        public SelectionInfo selection = new SelectionInfo();
        public RewiringInfo rewiring = new RewiringInfo();
        public CreationDialogInfo creationDialog = new CreationDialogInfo();

        public void ZoomAround(Vector2 pos, int delta)
        {
            float factor = (float)Math.Pow(1.001, delta);
            zoom *= factor;
            translate = From(Into(translate) - pos * (factor - 1f));
        }

        public void Move(Vector2 delta)
        {
            translate += delta * 1f / zoom;
        }

        public Vector2 Into(Vector2 pos)
        {
            return (pos + translate) * zoom;
        }

        public Vector2 From(Vector2 pos)
        {
            return pos / zoom - translate;
        }

        public Matrix3x2 Into()
        {
            return Matrix3x2.CreateTranslation(translate) * Matrix3x2.CreateScale(zoom);
        }

        public Matrix3x2 From()
        {
            return Matrix3x2.CreateScale(1f / zoom) * Matrix3x2.CreateTranslation(-translate);
        }
    }
}
