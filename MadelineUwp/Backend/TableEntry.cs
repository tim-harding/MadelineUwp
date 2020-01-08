namespace Madeline
{
    internal struct TableEntry<T>
    {
        public int id;
        public int row;
        public T value;

        public TableEntry(int id, int row, T value)
        {
            this.id = id;
            this.row = row;
            this.value = value;
        }
    }
}
