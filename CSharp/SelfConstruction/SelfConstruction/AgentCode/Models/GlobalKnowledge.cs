using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SelfConstruction.AgentCode.Models
{
    public class GlobalKnowledge
    {
        private static GlobalKnowledge _instance;

        private GlobalKnowledge() { }

        public static GlobalKnowledge Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GlobalKnowledge();
                }
                return _instance;
            }
        }

        public ConcurrentBag<Agent> Agents = new ConcurrentBag<Agent>();
        public ConcurrentBag<BuildingShape> Blocks = new ConcurrentBag<BuildingShape>();
        public ConcurrentBag<Pheromone> Pheromones = new ConcurrentBag<Pheromone>();
        public ConcurrentBag<BuildingShape> StepBlocks = new ConcurrentBag<BuildingShape>();
        public int SpacePheromoneCounter = 0;
    }
}