using System.Collections;
using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class Table<T> : IEnumerable<TableEntry<T>>
    {
        private int next;
        private List<int> ids = new List<int>();
        private List<T> values = new List<T>();

        public int Insert(T value)
        {
            int id = next++;
            ids.Add(id);
            values.Add(value);
            return id;
        }

        public void InsertWithId(int id, T value)
        {
            if (TryGetRowForId(id, out int row))
            {
                // Element with this ID already exists
                return;
            }
            int index = ~row;
            ids.Insert(index, id);
            values.Insert(index, value);
        }

        public void Update(int id, T value)
        {
            if (TryGetRowForId(id, out int row))
            {
                values[row] = value;
            }
        }

        public void UpdateAtRow(int row, T value)
        {
            values[row] = value;
        }

        public void Delete(int id)
        {
            if (TryGetRowForId(id, out int row))
            {
                ids.RemoveAt(row);
                values.RemoveAt(row);
            }
        }

        public bool TryGet(int id, out T value)
        {
            bool success = TryGetRowForId(id, out int index);
            value = success ? values[index] : default;
            return success;
        }

        public bool TryGetRowForId(int id, out int row)
        {
            row = ids.BinarySearch(id);
            return row > -1;
        }

        public T GetAtRow(int row)
        {
            return values[row];
        }

        public IEnumerator<TableEntry<T>> GetEnumerator()
        {
            for (int i = 0; i < ids.Count; i++)
            {
                yield return new TableEntry<T>(ids[i], i, values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
