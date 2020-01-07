using System.Numerics;

namespace Madeline.Backend
{
    internal struct Node
    {
        private const float WIDTH = 90f;
        private const float HEIGHT = 30f;
        private const float INPUT_SEPARATION = 35f;

        public Vector2 pos;
        public string name;
        public int plugin;

        public static Vector2 Size => new Vector2(WIDTH, HEIGHT);

        public Node(Vector2 pos, Plugin plugin, int pluginId)
        {
            this.pos = pos;
            this.plugin = pluginId;
            name = plugin.name;
        }

        public Vector2 InputPos(int input, int inputs)
        {
            float local = input - (inputs - 1) / 2f;
            var offset = new Vector2
            {
                X = WIDTH / 2f + local * INPUT_SEPARATION,
                Y = 0f,
            };
            return pos + offset;
        }

        public Vector2 OutputPos()
        {
            var offset = new Vector2
            {
                X = WIDTH / 2f,
                Y = HEIGHT,
            };
            return pos + offset;
        }
    }
}
