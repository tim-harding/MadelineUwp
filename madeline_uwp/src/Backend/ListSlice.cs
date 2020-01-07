using System.Collections;
using System.Collections.Generic;

namespace Madeline.Backend
{
    internal struct ListSlice<T> : IEnumerable<T>
    {
        private List<T> values;
        private Range range;

        public ListSlice(List<T> values, Range range)
        {
            this.values = values;
            this.range = range;
        }

        public T Consume()
        {
            return values[range.start++];
        }

        public T At(int i)
        {
            return values[range.start + i];
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = range.start; i < range.end; i++)
            {
                yield return values[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
