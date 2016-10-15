using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.MovementAlgorithm
{
    /// <summary>
    /// Class that represents the current situation of an Agent.
    /// </summary>
    public class State
    {
      /// <summary>
      /// The pheromone intensity
      /// </summary>
      public readonly Position PheromoneIntensity;

      /// <summary>
      /// Initializes a new instance of the <see cref="State"/> class.
      /// </summary>
      /// <param name="pheromoneIntensity">The pheromone intensity.</param>
      public State(Position pheromoneIntensity)
        {
            PheromoneIntensity = pheromoneIntensity;
        }

      /// <summary>
      /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
      /// </summary>
      /// <param name="obj">The object to compare with the current object.</param>
      /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
      public override bool Equals(object obj)
        {
            if (!(obj is State)) { return false; }

            return ((State)obj).PheromoneIntensity.Equals(PheromoneIntensity);
        }

      /// <summary>
      /// Returns a hash code for this instance.
      /// </summary>
      /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
      public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

      /// <summary>
      /// Returns a <see cref="System.String" /> that represents this instance.
      /// </summary>
      /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
      public override string ToString()
        {
            return string.Format("{0}", base.ToString());
        }
    }
}
