namespace Madeline
{
    internal struct TableRow<T>
    {
        public int id;
        public T value;

        public TableRow(int id, T value)
        {
            this.id = id;
            this.value = value;
        }
    }
}
