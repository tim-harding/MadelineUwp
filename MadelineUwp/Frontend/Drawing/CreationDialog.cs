using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline.Frontend.Drawing
{
    internal class CreationDialog : IDrawer
    {
        private const float WIDTH = 120f;
        private const float MARGIN = 10f;
        private const float LINE_HEIGHT = 38f;

        private Vector2 Margin = Vector2.UnitX * MARGIN;
        private Vector2 Line = Vector2.UnitY * LINE_HEIGHT;

        private Handlers.CreationDialog dialog;
        private CanvasDrawingSession session;

        public CreationDialog(Handlers.CreationDialog dialog)
        {
            this.dialog = dialog;
        }

        public void Draw(CanvasDrawingSession session)
        {
            session.Transform = Matrix3x2.CreateTranslation(dialog.origin);
            this.session = session;
            if (!dialog.display) { return; }

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

            session.DrawImage(comp);
        }

        private ICanvasImage SolidBackground()
        {
            const float ROUNDING = 5f;
            int lines = dialog.found.Count + 1;
            var size = new Vector2(WIDTH, lines * LINE_HEIGHT);
            var rect = new Rect(Vector2.Zero.ToPoint(), size.ToSize());
            var clip = CanvasGeometry.CreateRoundedRectangle(session.Device, rect, ROUNDING, ROUNDING);
            var target = new CanvasRenderTarget(session.Device, size.X, size.Y, session.Dpi);
            using (CanvasDrawingSession offscreen = target.CreateDrawingSession())
            {
                using (offscreen.CreateLayer(1f, clip))
                {
                    CanvasDrawingSession tmp = session;
                    session = offscreen;
                    session.Clear(Palette.Tone5);
                    FillLine(0, Palette.Tone6);
                    DrawSelection();
                    session = tmp;
                }
            }
            return target;
        }

        private void DrawDivisions()
        {
            var halfLine = new Vector2(0f, 0.5f);
            int lines = dialog.found.Count + 1;
            for (int i = 1; i < lines; i++)
            {
                Vector2 line = Line * i;
                Vector2 start = line + halfLine;
                Vector2 end = line + Vector2.UnitX * WIDTH + halfLine;
                session.DrawLine(start, end, Palette.Tone8);
            }
        }

        private void DrawSelection()
        {
            int selection = dialog.selection;
            if (selection > -1)
            {
                FillLine(selection + 1, Palette.Teal7);
            }
        }

        private void FillLine(int line, Color color)
        {
            Vector2 offset = Line * line;
            Vector2 size = Line + Vector2.UnitX * WIDTH;
            var rect = new Rect(offset.ToPoint(), size.ToSize());
            session.FillRectangle(rect, color);
        }

        private void DrawQuery()
        {
            string query = dialog.query;
            int failPoint = dialog.failPoint;
            failPoint = failPoint < 0 ? query.Length : failPoint;
            string valid = query.Substring(0, failPoint);
            string invalid = query.Substring(failPoint);
            CanvasTextLayout layout = LineLayout(valid);
            session.DrawTextLayout(layout, Margin, Colors.White);
            Vector2 offset = Vector2.UnitX * (float)layout.LayoutBounds.Width;
            layout = LineLayout(invalid);
            session.DrawTextLayout(layout, Margin + offset, Palette.Red5);
        }

        private void DrawFound()
        {
            for (int i = 0; i < dialog.found.Count; i++)
            {
                Vector2 offset = (i + 1) * Line + Margin;
                CanvasTextLayout layout = LineLayout(dialog.found[i]);
                session.DrawTextLayout(layout, offset, Palette.White);
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
            var layout = new CanvasTextLayout(session.Device, text, format, WIDTH, LINE_HEIGHT);
            return layout;
        }
    }
}
