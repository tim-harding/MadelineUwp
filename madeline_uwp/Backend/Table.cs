using System.Collections;
using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class Table<T> : IEnumerable<TableRow<T>>
    {
        private int next;
        private List<int> ids = new List<int>();
        private List<T> values = new List<T>();

        public void Insert(T value)
        {
            int id = next++;
            ids.Add(id);
            values.Add(value);
        }

        public void Update(int id, T value)
        {
            int index = ids.BinarySearch(id);
            if (index >= 0)
            {
                values[index] = value;
            }
        }

        public void Delete(int id)
        {
            int index = ids.BinarySearch(id);
            if (index >= 0)
            {
                ids.RemoveAt(index);
                values.RemoveAt(index);
            }
        }

        public bool TryGet(int id, out T value)
        {
            int index = ids.BinarySearch(id);
            bool success = index >= 0;
            value = success ? values[index] : default;
            return success;
        }

        public T Get(int id)
        {
            int index = ids.BinarySearch(id);
            return values[index];
        }

        public IEnumerator<TableRow<T>> GetEnumerator()
        {
            for (int i = 0; i < ids.Count; i++)
            {
                yield return new TableRow<T>(ids[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
