using System.Collections.Generic;

namespace Madeline.Backend
{
    internal class RangeTable<T>
    {
        private Table<Range> indices = new Table<Range>();
        private List<T> values = new List<T>();

        public void Extend(int count, T template)
        {
            var range = new Range(values.Count, values.Count + count);
            indices.Insert(range);
            for (int i = 0; i < count; i++)
            {
                values.Add(template);
            }
        }

        public void Insert(IEnumerable<T> multiple)
        {
            int start = values.Count;
            values.AddRange(multiple);
            indices.Insert(new Range(start, values.Count));
        }

        public void Update(int id, int offset, T value)
        {
            if (indices.TryGet(id, out Range range) && offset < range.Count())
            {
                values[range.start + offset] = value;
            }
        }

        public void Delete(int id)
        {
            if (indices.TryGet(id, out Range range))
            {
                int count = range.Count();
                values.RemoveRange(range.start, count);
                indices.Delete(id);
                foreach (TableRow<Range> row in indices)
                {
                    if (row.id > id)
                    {
                        Range update = row.value;
                        update.start -= count;
                        update.end -= count;
                        indices.Update(row.id, update);
                    }
                }
            }
        }

        public bool TryGet(int id, out ListSlice<T> slice)
        {
            if (indices.TryGet(id, out Range range))
            {
                slice = new ListSlice<T>(values, range);
                return true;
            }
            slice = new ListSlice<T>(values, new Range(0, 0));
            return false;
        }

        public ListSlice<T> Get(int id)
        {
            if (indices.TryGet(id, out Range range))
            {
                return new ListSlice<T>(values, range);
            }
            return new ListSlice<T>(values, new Range(0, 0));
        }
    }
}
