using System.Collections.Concurrent;

namespace SelfConstruction.AgentCode.Models
{
   /// <summary>
   /// Class GlobalKnowledge.
   /// </summary>
   public class GlobalKnowledge
   {
      /// <summary>
      /// This contains the Singleton instance
      /// </summary>
      private static GlobalKnowledge _instance;

      /// <summary>
      /// Prevents a default instance of the <see cref="GlobalKnowledge"/> class from being created.
      /// </summary>
      private GlobalKnowledge()
      {
      }

      /// <summary>
      /// Gets the instance.
      /// </summary>
      /// <value>The instance.</value>
      public static GlobalKnowledge Instance
      {
         get { return _instance ?? (_instance = new GlobalKnowledge()); }
      }

      /// <summary>
      /// Contains the Agents
      /// </summary>
      public ConcurrentBag<Agent> Agents = new ConcurrentBag<Agent>();

      /// <summary>
      /// Contains the already built blocks
      /// </summary>
      public ConcurrentBag<BuildingShape> Blocks = new ConcurrentBag<BuildingShape>();

      /// <summary>
      /// The current build pheromones
      /// </summary>
      public ConcurrentBag<Pheromone> BuildPheromones = new ConcurrentBag<Pheromone>();

      /// <summary>
      /// The current space pheromones
      /// </summary>
      public ConcurrentBag<Pheromone> SpacePheromones = new ConcurrentBag<Pheromone>();

      /// <summary>
      /// The Blocks which are build in the last iteration
      /// </summary>
      public ConcurrentBag<BuildingShape> StepBlocks = new ConcurrentBag<BuildingShape>();

      /// <summary>
      /// The initial pheromone (At the moment always just one)
      /// </summary>
      public Pheromone InitialPheromone = new Pheromone(50, 0, Pheromonetype.Initial, new Position(0, 0, 0));

      /// <summary>
      /// The space pheromone counter
      /// </summary>
      public int SpacePheromoneCounter = 0;
   }
}