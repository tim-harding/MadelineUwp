using Madeline.Frontend.Layout;
using Madeline.Frontend.Panes.Controls.Drawing;
using Madeline.Frontend.Panes.NodeGraph.Behavior;
using Madeline.Frontend.Panes.NodeGraph.Drawing;
using Madeline.Frontend.Panes.NodeGraph.Structure;

namespace Madeline.Frontend.Panes
{
    internal static class Factory
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

        public static Pane ControlsPane()
        {
            var handlers = new IInputHandler[]
            {

            };

            var drawers = new IDrawer[]
            {
                new ControlsDrawer(),
            };

            return new Pane(handlers, drawers);
        }
    }
}
