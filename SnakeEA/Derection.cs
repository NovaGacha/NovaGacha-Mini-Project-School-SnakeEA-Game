
namespace SnakeEA
{
    public class Derection
    {
        public readonly static Derection left = new Derection(0, -1);
        public readonly static Derection right = new Derection(0, 1);
        public readonly static Derection Up = new Derection(-1, 0);
        public readonly static Derection Down = new Derection(1, 0);
        public int RowOffset { get; }
        public int ColOffset { get; }

        private Derection(int rowOffset, int colOffset)
        {
            RowOffset = rowOffset;
            ColOffset = colOffset;
        }
        public Derection Opposite() 
        {
            return new Derection (-RowOffset, -ColOffset);

        }

        public override bool Equals(object obj)
        {
            return obj is Derection derection &&
                   RowOffset == derection.RowOffset &&
                   ColOffset == derection.ColOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Derection left, Derection right)
        {
            return EqualityComparer<Derection>.Default.Equals(left, right);
        }

        public static bool operator !=(Derection left, Derection right)
        {
            return !(left == right);
        }
    }
}
