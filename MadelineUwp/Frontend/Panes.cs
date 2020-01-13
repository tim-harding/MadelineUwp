using Madeline.Frontend.Drawing;
using Madeline.Frontend.Drawing.Graph;
using Madeline.Frontend.Handlers;
using Madeline.Frontend.Handlers.Graph;
using Madeline.Frontend.Layout;

namespace Madeline.Frontend
{
    internal static class Panes
    {
        public static Pane NodeGraphPane()
        {
            var viewport = new Viewport();

            var handlers = new IInputHandler[]
            {
                new CreationDialogHandler(viewport),
                new NodesHandler(viewport),
                new DragSelectHandler(viewport),
                new WireCreationHandler(viewport),
            };

            var drawers = new IDrawer[]
            {
                new NodesDrawer(viewport),
                new WireCreationDrawer(viewport),
                new CreationDialogDrawer(viewport),
            };

            return new Pane(handlers, drawers);
        }
    }
}
