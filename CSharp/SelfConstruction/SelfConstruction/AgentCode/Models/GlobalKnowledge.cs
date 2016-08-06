using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SelfConstruction.AgentCode.Models
{
    public struct GlobalKnowledge
    {
        public ConcurrentBag<Agent> Agents;
        public ConcurrentBag<BuildingShape> Blocks;
        public ConcurrentBag<Pheromone> Pheromones;
    }
}