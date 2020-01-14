using Madeline.Frontend.Panes.NodeGraph.Behavior;
using Madeline.Frontend.Panes.NodeGraph.Drawing;
using Madeline.Frontend.Panes.NodeGraph.Structure;
using Madeline.Frontend.Panes.Viewer;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace Madeline
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            InitNodeGraph();
            InitImageView();
        }

        private void InitNodeGraph()
        {
            var viewport = new Viewport();
            nodeGraph.handlers = new Frontend.IInputHandler[]
            {
                new CreationDialogHandler(viewport),
                new NodesHandler(viewport),
                new WireCreationHandler(viewport),
                new DragSelectHandler(viewport),
            };
            nodeGraph.drawers = new Frontend.IDrawer[]
            {
                new NodesDrawer(viewport),
                new WireCreationDrawer(viewport),
                new CreationDialogDrawer(viewport),
            };
        }

        private void InitImageView()
        {
            view.handlers = new Frontend.IInputHandler[]
            {
                new ViewerHandler(),
            };
            view.drawers = new Frontend.IDrawer[]
            {
                new ViewerDrawer(),
            };
        }
    }
}
