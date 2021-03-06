﻿using System;
using System.Linq;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
   /// <summary>
   /// Class Utils.
   /// </summary>
   public class Utils
    {
      /// <summary>
      /// Prevents a default instance of the <see cref="Utils"/> class from being created.
      /// </summary>
      private Utils() { }

      /// <summary>
      /// Gets the instance of <see cref="Utils"/> class.
      /// </summary>
      /// <value>The instance.</value>
      public static Utils Instance { get; } = new Utils();

        /// <summary>
        /// Calculates the distance to a specific brick.
        /// </summary>
        /// <param name="position1">The position1.</param>
        /// <param name="position2">The position2.</param>
        /// <returns>System.Double.</returns>
        public double CalculateDistanceToBrick(Position? position1, Position position2)
        {
            if (position1 == null)
            {
                return 1;
            }

            Position position = (Position) position1;

            var deltaX = position.X - position2.X;
            var deltaY = position.Y - position2.Y;
            var deltaZ = position.Z - position2.Z;

            return Math.Sqrt(deltaX*deltaX + deltaY*deltaY + deltaZ*deltaZ);
        }

        /// <summary>
        /// Determines whether [is position free] [the specified global knowledge].
        /// </summary>
        /// <param name="positionToCheck">The position to check.</param>
        /// <returns><c>true</c> if [is position free] [the specified global knowledge]; otherwise, <c>false</c>.</returns>
        public bool IsPositionFree(Position positionToCheck)
        {
            var items = GlobalKnowledge.Instance.Blocks.Select(buildingshape => buildingshape.Position).ToList()
                .Concat(GlobalKnowledge.Instance.Agents.Select(agent => agent.Position).ToList());

            return !items.Contains(positionToCheck);
            
        }

        /// <summary>
        /// Determines whether [is position equal] [the specified position1].
        /// </summary>
        /// <param name="position1">The position1.</param>
        /// <param name="position2">The position2.</param>
        /// <returns><c>true</c> if [is position equal] [the specified position1]; otherwise, <c>false</c>.</returns>
        public bool IsPositionEqual(Position position1 , Position position2 )
        {
            return position1.Equals(position2);
        }

        /// <summary>
        /// Removes the vaporated pheromones.
        /// </summary>
        /// <param name="pheromones">The pheromones.</param>
        public void RemoveVaporatedPheromones()
        {
            foreach (Pheromone pheromone in GlobalKnowledge.Instance.BuildPheromones)
            {
                if (pheromone.Intensity < 2)
                {
                    Pheromone t = pheromone;
                    GlobalKnowledge.Instance.BuildPheromones.TryTake(out t);
                }
            }
        }
    }
}