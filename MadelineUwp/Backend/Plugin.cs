using Windows.UI;

namespace Madeline.Backend
{
    internal struct Plugin
    {
        public string name;
        public int inputs;
        public ColorScheme colors;

        public Plugin(string name, int inputs, ColorScheme colors)
        {
            this.name = name;
            this.inputs = inputs;
            this.colors = colors;
        }

        public struct ColorScheme
        {
            public Color body;
            public Color hover;

            public ColorScheme(Color body, Color hover)
            {
                this.body = body;
                this.hover = hover;
            }
        }
    }
}
