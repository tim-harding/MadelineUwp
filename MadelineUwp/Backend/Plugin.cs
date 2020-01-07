namespace Madeline.Backend
{
    internal struct Plugin
    {
        public string name;
        public int inputs;

        public Plugin(string name, int inputs)
        {
            this.name = name;
            this.inputs = inputs;
        }
    }
}
