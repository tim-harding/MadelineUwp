using System.Numerics;

namespace Madeline.Backend
{
    internal class Node
    {
        private const float WIDTH = 90f;
        private const float HEIGHT = 27f;
        private const float INPUT_SEPARATION = 35f;
        private const float SLOT_OFFSET = 7f;

        public Vector2 pos;
        public string name;
        public Plugin plugin;
        public bool enabled;
        public int[] inputs;

        public static Vector2 Size => new Vector2(WIDTH, HEIGHT);

        public Node(Vector2 pos, Plugin plugin)
        {
            this.pos = pos;
            this.plugin = plugin;
            name = plugin.name;
            inputs = new int[plugin.inputs];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = -1;
            }
            enabled = true;
        }

        public Vector2 SlotPos(int slot, int inputs)
        {
            if (slot < 0)
            {
                return OutputPos();
            }
            else
            {
                return InputPos(slot, inputs);
            }
        }

        public Vector2 InputPos(int input, int inputs)
        {
            float local = input - (inputs - 1) / 2f;
            var offset = new Vector2
            {
                X = WIDTH / 2f + local * INPUT_SEPARATION,
                Y = -SLOT_OFFSET,
            };
            return pos + offset;
        }

        public Vector2 OutputPos()
        {
            var offset = new Vector2
            {
                X = WIDTH / 2f,
                Y = HEIGHT + SLOT_OFFSET,
            };
            return pos + offset;
        }
    }
}
