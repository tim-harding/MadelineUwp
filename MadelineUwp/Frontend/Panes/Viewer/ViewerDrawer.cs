namespace Madeline.Frontend.Panes.Viewer
{
    internal class ViewerDrawer : IDrawer
    {
        public void Draw()
        {
            Globals.session.Clear(Palette.Green8);
        }
    }
}
