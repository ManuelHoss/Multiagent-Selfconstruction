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
        bool ShouldBuild(GlobalKnowledge globalKnowledge, Agent agent);
        bool ShouldRemove(GlobalKnowledge globalKnowledge, Agent agent);
        bool ShouldPlaceSpacePheromone(GlobalKnowledge globalKnowledge, Agent agent);
    }
}
