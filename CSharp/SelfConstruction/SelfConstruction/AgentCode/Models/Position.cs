namespace SelfConstruction.AgentCode.Models
{
    public struct Position
    {
        public int X;
        public int Y;
        public int Z;

        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Position)) { return false; }

            return ((Position)obj).X == X && ((Position)obj).Y == Y && ((Position)obj).Z == Z;
        }
    }
}