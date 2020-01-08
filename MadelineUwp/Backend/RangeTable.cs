using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class RangeTable<T>
    {
        private int next;
        private List<int> ids = new List<int>();
        private List<int> starts = new List<int>();
        private List<T> values = new List<T>();

        public void Extend(int count, T template)
        {
            ids.Add(next++);
            starts.Add(values.Count);
            for (int i = 0; i < count; i++)
            {
                values.Add(template);
            }
        }

        public void Insert(IEnumerable<T> multiple)
        {
            ids.Add(next++);
            starts.Add(values.Count);
            values.AddRange(multiple);
        }

        public void Update(int id, int offset, T value)
        {
            if (TryGetRange(id, out Range range) && offset < range.Count)
            {
                values[range.start + offset] = value;
            }
        }

        public void UpdateAtRow(int row, int offset, T value)
        {
            values[row + offset] = value;
        }

        public void Delete(int id)
        {
            if (TryGetRowForId(id, out int row))
            {
                int start = starts[row];
                int end = row < starts.Count - 1 ? starts[row + 1] : values.Count;
                int count = end - start;
                for (int i = row; i < starts.Count; i++)
                {
                    starts[i] -= count;
                }
                ids.RemoveAt(row);
                starts.RemoveAt(row);
                values.RemoveRange(start, count);
            }
        }

        public bool TryGet(int id, out ListSlice<T> slice)
        {
            if (TryGetRange(id, out Range range))
            {
                slice = new ListSlice<T>(values, range);
                return true;
            }
            slice = default;
            return false;
        }

        public bool TryGetRowForId(int id, out int row)
        {
            row = ids.BinarySearch(id);
            return row > -1;
        }

        public ListSlice<T> GetAtRow(int row)
        {
            return new ListSlice<T>(values, RangeForIndex(row));
        }

        private bool TryGetRange(int id, out Range range)
        {
            int index = ids.BinarySearch(id);
            if (index > -1)
            {
                range = RangeForIndex(index);
                return true;
            }
            range = default;
            return false;
        }

        private Range RangeForIndex(int index)
        {
            int start = starts[index];
            int end = index < starts.Count - 1 ? starts[index + 1] : starts.Count;
            return new Range(start, end);
        }
    }
}
