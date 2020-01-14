using Madeline.Backend;
using System.Numerics;

namespace Madeline.Frontend.Panes.Controls.Drawing
{
    internal class ControlsDrawer : IDrawer
    {
        private const float LINE_HEIGHT = 32f;
        private const float MARGIN = 6f;
        private const float NODE_NAME_WIDTH = 60f;

        private Vector2 Line = Vector2.UnitY * LINE_HEIGHT;
        private Vector2 Margin = Vector2.UnitX * MARGIN;

        public void Draw()
        {
            if (!Globals.graph.nodes.TryGet(Globals.graph.active, out Node node)) { return; }

            for (int i = 0; i < node.controls.Length; i++)
            {
                Control control = node.controls[i];
                Vector2 origin = Line * i + Margin;
                Globals.session.DrawText(control.name, origin, Palette.White);
                origin += Vector2.UnitX * NODE_NAME_WIDTH;

                switch (control)
                {
                    case TextControl text:
                        break;

                    case IntegerControl integer:
                        break;

                    case RealControl real:
                        break;
                }
            }
        }
    }
}
