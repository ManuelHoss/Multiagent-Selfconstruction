using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.Interfaces
{
    public interface IPheromoneModel
    {
        bool ShouldBuild(Position position);
        bool ShouldRemove(Position cubePosition);
        bool ShouldPlaceSpacePheromone(Agent agent);
    }
}
