using Windows.UI;

namespace Madeline.Backend
{
    internal class Plugin
    {
        public string name;
        public int inputs;
        public ColorScheme colors;
        public Control[] controls;

        public Plugin() { }

        public Plugin(string name, int inputs, ColorScheme colors, Control[] controls)
        {
            this.name = name;
            this.inputs = inputs;
            this.colors = colors;
            this.controls = controls;
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
