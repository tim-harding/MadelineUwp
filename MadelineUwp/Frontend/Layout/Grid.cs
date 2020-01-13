using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace Madeline.Frontend.Layout
{
    internal class Grid
    {
        private List<float> rows = new List<float>();
        private List<float> columns = new List<float>();

        public Vector2 dimensions;

        public Grid(float[] rows, float[] columns)
        {
            InitList(this.rows, rows);
            InitList(this.columns, columns);
        }

        public Rect Occupant(Occupant occupant)
        {
            float left = rows[occupant.rows.start];
            float right = rows[occupant.rows.end + 1];
            float top = columns[occupant.columns.start];
            float bottom = columns[occupant.columns.end + 1];
            var pos = new Vector2(left, top);
            pos *= dimensions;
            var size = new Vector2(right - left, bottom - top);
            size *= dimensions;
            return new Rect(pos.ToPoint(), size.ToSize());
        }

        private void InitList(List<float> member, float[] source)
        {
            float sum = Sum(source);
            float acc = 0f;
            for (int i = 0; i < source.Length; i++)
            {
                member.Add(acc);
                acc += source[i] / sum;
            }
            member.Add(acc);
            Normalize(member);
        }

        private void Normalize(List<float> values)
        {
            float rowSum = Sum(values);
            for (int i = 0; i < values.Count; i++)
            {
                values[i] /= rowSum;
            }
        }

        private float Sum(IEnumerable<float> values)
        {
            float sum = 0f;
            foreach (float value in values)
            {
                sum += value;
            }
            return sum;
        }
    }
}
