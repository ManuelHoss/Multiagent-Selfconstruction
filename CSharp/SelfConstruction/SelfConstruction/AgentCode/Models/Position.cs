namespace SelfConstruction.AgentCode.Models
{
   /// <summary>
   /// This represents a basic Position in a 3D-Area (used for every step and build)
   /// </summary>
   public struct Position
   {
      /// <summary>
      /// The x coordinate
      /// </summary>
      public int X;

      /// <summary>
      /// The y coordinate
      /// </summary>
      public int Y;

      /// <summary>
      /// The z coordinate
      /// </summary>
      public int Z;

      /// <summary>
      /// Initializes a new instance of the <see cref="Position"/> struct.
      /// </summary>
      /// <param name="x">The x coordinate.</param>
      /// <param name="y">The y coordinate.</param>
      /// <param name="z">The z coordinate.</param>
      public Position(int x, int y, int z)
      {
         X = x;
         Y = y;
         Z = z;
      }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
      /// </summary>
      /// <param name="obj">The object to compare with the current instance.</param>
      /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
      public override bool Equals(object obj)
      {
         if (!(obj is Position))
         {
            return false;
         }

         return ((Position) obj).X == X && ((Position) obj).Y == Y && ((Position) obj).Z == Z;
      }
   }
}