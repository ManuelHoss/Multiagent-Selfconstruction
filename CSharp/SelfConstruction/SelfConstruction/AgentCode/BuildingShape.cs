using System;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
    /// <summary>
    /// Struct BuildingShape
    /// </summary>
    public struct BuildingShape
    {
      /// <summary>
      /// The position of the BuildingSpape.
      /// </summary>
      public Position Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingShape"/> struct.
        /// </summary>
        /// <param name="position">The position of the <see cref="BuildingShape"/>.</param>
        /// <param name="pheromone">The pheromone.</param>
        public BuildingShape(Position position, Pheromone? pheromone = null)
        {
            Position = position;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format("BuildingShape at Position: ({0}, {1}, {2})", Position.X, Position.Y, Position.Z);
        }
    }
}
