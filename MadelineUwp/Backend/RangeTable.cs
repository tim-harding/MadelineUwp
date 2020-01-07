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

        public void Delete(int id)
        {
            int index = ids.BinarySearch(id);
            if (index > -1)
            {
                int start = starts[index];
                int end = index < starts.Count - 1 ? starts[index + 1] : values.Count;
                int count = end - start;
                for (int i = index; i < starts.Count; i++)
                {
                    starts[i] -= count;
                }
                ids.RemoveAt(index);
                starts.RemoveAt(index);
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

        public ListSlice<T> Get(int id)
        {
            if (TryGetRange(id, out Range range))
            {
                return new ListSlice<T>(values, range);
            }
            return new ListSlice<T>(values, new Range(0, 0));
        }

        private bool TryGetRange(int id, out Range range)
        {
            int index = ids.BinarySearch(id);
            if (index > -1)
            {
                int start = starts[index];
                int end = index < starts.Count - 1 ? starts[index + 1] : starts.Count;
                range = new Range(start, end);
                return true;
            }
            range = default;
            return false;
        }
    }
}
