namespace Madeline.Backend
{
    internal abstract class Control
    {
        public string name;
    }

    internal class GenericControl<T> : Control
    {
        public T initial;

        public GenericControl(string name, T initial)
        {
            this.name = name;
            this.initial = initial;
        }
    }

    internal class TextControl : GenericControl<string>
    {
        public TextControl(string name, string initial) : base(name, initial) { }
    }

    internal class FloatControl : GenericControl<float>
    {
        public FloatControl(string name, float initial) : base(name, initial) { }
    }

    internal class IntControl : GenericControl<int>
    {
        public IntControl(string name, int initial) : base(name, initial) { }
    }
}
