using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SelfConstruction.AgentCode.Models
{
    public class GlobalKnowledge
    {
        private static GlobalKnowledge _instance;

        private GlobalKnowledge()
        {
        }

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
        public ConcurrentBag<Pheromone> BuildPheromones = new ConcurrentBag<Pheromone>();
        public ConcurrentBag<Pheromone> SpacePheromones = new ConcurrentBag<Pheromone>();
        public ConcurrentBag<BuildingShape> StepBlocks = new ConcurrentBag<BuildingShape>();
        public Pheromone InitialPheromone = new Pheromone(50, 0, Pheromonetype.Initial, new Position(0, 0, 0));

        public int SpacePheromoneCounter = 0;
    }
}