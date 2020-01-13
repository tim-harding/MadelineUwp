using Madeline.Frontend.Handlers.Graph;
using Madeline.Frontend.Structure;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline.Frontend.Drawing.Graph
{
    internal class CreationDialogDrawer : IDrawer
    {
        private Vector2 Width = Vector2.UnitX * CreationDialogHandler.WIDTH;
        private Vector2 Margin = Vector2.UnitX * CreationDialogHandler.MARGIN;
        private Vector2 Line = Vector2.UnitY * CreationDialogHandler.LINE_HEIGHT;

        private CreationDialogInfo info;

        public CreationDialogDrawer(Viewport viewport)
        {
            info = viewport.creationDialog;
        }

        public void Draw()
        {
            Globals.session.Transform = Matrix3x2.CreateTranslation(info.origin);
            if (!info.display) { return; }

            DrawBackground();
            DrawDivisions();
            DrawQuery();
            DrawFound();
        }

        private void DrawBackground()
        {
            ICanvasImage solid = SolidBackground();

            var shadow = new ShadowEffect
            {
                Source = solid,
                BlurAmount = 4f,
                ShadowColor = Color.FromArgb(64, 0, 0, 0),
            };

            var offset = new Transform2DEffect
            {
                TransformMatrix = Matrix3x2.CreateTranslation(0f, 4f),
                Source = shadow,
            };

            var comp = new CompositeEffect
            {
                Sources = { offset, solid },
            };

            Globals.session.DrawImage(comp);
        }

        private ICanvasImage SolidBackground()
        {
            const float ROUNDING = 5f;
            int lines = info.found.Count + 1;
            Vector2 size = Width + Line * lines;
            var rect = new Rect(Vector2.Zero.ToPoint(), size.ToSize());
            var clip = CanvasGeometry.CreateRoundedRectangle(Globals.session.Device, rect, ROUNDING, ROUNDING);
            var target = new CanvasRenderTarget(Globals.session.Device, size.X, size.Y, Globals.session.Dpi);
            using (CanvasDrawingSession offscreen = target.CreateDrawingSession())
            {
                using (offscreen.CreateLayer(1f, clip))
                {
                    CanvasDrawingSession tmp = Globals.session;
                    Globals.session = offscreen;
                    Globals.session.Clear(Palette.Tone5);
                    FillLine(0, Palette.Tone6);
                    DrawSelection();
                    Globals.session = tmp;
                }
            }
            return target;
        }

        private void DrawDivisions()
        {
            var halfLine = new Vector2(0f, 0.5f);
            int lines = info.found.Count + 1;
            for (int i = 1; i < lines; i++)
            {
                Vector2 line = Line * i;
                Vector2 start = line + halfLine;
                Vector2 end = line + Width + halfLine;
                Globals.session.DrawLine(start, end, Palette.Tone8);
            }
        }

        private void DrawSelection()
        {
            int selection = info.selection;
            if (selection > -1)
            {
                FillLine(selection + 1, Palette.Teal7);
            }
        }

        private void FillLine(int line, Color color)
        {
            Vector2 offset = Line * line;
            Vector2 size = Line + Width;
            var rect = new Rect(offset.ToPoint(), size.ToSize());
            Globals.session.FillRectangle(rect, color);
        }

        private void DrawQuery()
        {
            string query = info.query;
            int failPoint = info.failPoint;
            failPoint = failPoint < 0 ? query.Length : failPoint;
            string valid = query.Substring(0, failPoint);
            string invalid = query.Substring(failPoint);
            CanvasTextLayout layout = LineLayout(valid);
            Globals.session.DrawTextLayout(layout, Margin, Colors.White);
            Vector2 offset = Vector2.UnitX * (float)layout.LayoutBounds.Width;
            layout = LineLayout(invalid);
            Globals.session.DrawTextLayout(layout, Margin + offset, Palette.Red5);
        }

        private void DrawFound()
        {
            for (int i = 0; i < info.found.Count; i++)
            {
                Vector2 offset = (i + 1) * Line + Margin;
                CanvasTextLayout layout = LineLayout(info.found[i]);
                Globals.session.DrawTextLayout(layout, offset, Palette.White);
            }
        }

        private CanvasTextLayout LineLayout(string text)
        {
            var format = new CanvasTextFormat
            {
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = 18f,
                WordWrapping = CanvasWordWrapping.NoWrap,
            };
            var layout = new CanvasTextLayout(Globals.session.Device, text, format, Width.X, Line.Y);
            return layout;
        }
    }
}
