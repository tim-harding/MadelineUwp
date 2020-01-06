using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Madeline
{
    internal class NodeCreationDialogDrawer
    {
        private const int MARGIN = 6;

        private Vector2 Margin = Vector2.UnitX * MARGIN;

        private NodeCreationDialog dialog;
        private CanvasDrawingSession session;

        public NodeCreationDialogDrawer(NodeCreationDialog dialog)
        {
            this.dialog = dialog;
        }

        public void Draw(CanvasDrawingSession session)
        {
            this.session = session;
            if (!dialog.display)
            {
                return;
            }

            DrawBackground();
            DrawQuery();
            DrawSeperator();
            DrawSelection();

            int maxCount = NodeCreationDialog.HEIGHT / NodeCreationDialog.LEADING;
            int count = Math.Min(dialog.found.Count, maxCount);
            for (int i = 0; i < count; i++)
            {
                DrawLine(i);
            }
        }

        private void DrawBackground()
        {
            var rect = new Rect(dialog.origin.ToPoint(), dialog.Size.ToSize());
            session.FillRectangle(rect, Color.FromArgb(255, 48, 48, 48));
        }

        private void DrawSeperator()
        {
            Vector2 line = dialog.Line;
            session.DrawLine(line, line + Vector2.UnitX * dialog.Size.X, Colors.White);
        }

        private void DrawSelection()
        {
            int selection = dialog.selection;
            if (selection > 0)
            {
                float line = selection + 1f;
                Vector2 upperLeft = dialog.origin + Vector2.UnitY * dialog.Line * line;
                var rect = new Rect(upperLeft.ToPoint(), dialog.Size.ToSize());
                session.FillRectangle(rect, Color.FromArgb(255, 64, 64, 64));
            }
        }

        private void DrawLine(int i)
        {
            string name = dialog.found[i].plugin.name;
            Vector2 pos = dialog.origin + dialog.Line * i;
            session.DrawText(name, pos + Margin, Colors.White);
        }

        private void DrawQuery()
        {
            Vector2 origin = dialog.origin;
            Vector2 margin = Vector2.UnitX * MARGIN;
            string query = dialog.query;
            int failPoint = dialog.failPoint;
            failPoint = failPoint < 0 ? query.Length : failPoint;
            string valid = query.Substring(0, failPoint);
            string invalid = query.Substring(failPoint);
            var format = new CanvasTextFormat();
            var layout = new CanvasTextLayout(session.Device, valid, format, dialog.Size.X, dialog.Size.Y);
            Vector2 offset = Vector2.UnitX * (float)layout.LayoutBounds.Width;
            session.DrawTextLayout(layout, origin + margin, Colors.White);
            session.DrawText(invalid, origin + margin + offset, Colors.Red);
        }
    }
}
