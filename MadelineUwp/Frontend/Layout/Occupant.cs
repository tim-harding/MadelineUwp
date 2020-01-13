namespace Madeline.Frontend.Layout
{
    internal struct Occupant
    {
        public Range rows;
        public Range columns;
        public Pane pane;

        public Occupant(Range rows, Range columns, Pane pane)
        {
            this.rows = rows;
            this.columns = columns;
            this.pane = pane;
        }
    }
}
