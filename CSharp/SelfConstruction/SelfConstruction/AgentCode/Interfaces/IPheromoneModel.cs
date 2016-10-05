using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.Interfaces
{
    /// <summary>
    /// This is the Interface contains the PheromoneModels
    /// </summary>
    public interface IPheromoneModel
    {
        /// <summary>
        /// This Pheromone defines whether an agent should build or not
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        bool ShouldBuild(Position position);
        /// <summary>
        /// This Pheromone defines whether an agent should remove or not
        /// </summary>
        /// <param name="cubePosition">The cube position.</param>
        /// <returns></returns>
        bool ShouldRemove(Position cubePosition);
        /// <summary>
        /// This Pheromone defines whether an agent should place the SpacePheromone or not
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <returns></returns>
        bool ShouldPlaceSpacePheromone(Agent agent);
    }
}
