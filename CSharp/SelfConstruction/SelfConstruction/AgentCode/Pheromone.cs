using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
    /// <summary>
    /// Basic struct for Pheromones which contains all the properties of that object
    /// </summary>
    public struct Pheromone
    {
      /// <summary>
      /// The intensity
      /// </summary>
      public double Intensity { get; set; }
      /// <summary>
      /// The evaporation rate
      /// </summary>
      public double VaporationRate { get; set; }
      /// <summary>
      /// The pheromonetype
      /// </summary>
      public Pheromonetype Pheromonetype { get; set; }
      /// <summary>
      /// The position of the Pheromone
      /// </summary>
      public Position Position { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="Pheromone"/> struct.
      /// </summary>
      /// <param name="intensity">The intensity.</param>
      /// <param name="vaporationRate">The vaporation rate.</param>
      /// <param name="pheromonetype">The pheromonetype.</param>
      /// <param name="position">The position.</param>
      public Pheromone(double intensity = 50, double vaporationRate = 0.0000001, 
            Pheromonetype pheromonetype = Pheromonetype.Neutral, Position? position = null)
        {
            Intensity = intensity;
            VaporationRate = vaporationRate;
            Pheromonetype = pheromonetype;
            Position = position ?? new Position(0, 0, 0);
        }
    }
}
