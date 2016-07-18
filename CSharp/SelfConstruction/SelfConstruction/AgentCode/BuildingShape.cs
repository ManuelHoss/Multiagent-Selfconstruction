using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
    public struct BuildingShape
    {
        public Position Position;
        public Pheromone? Pheromone;

        public BuildingShape(Position position, Pheromone? pheromone = null)
        {
            Position = position;
            Pheromone = pheromone;
        }
    }
}
