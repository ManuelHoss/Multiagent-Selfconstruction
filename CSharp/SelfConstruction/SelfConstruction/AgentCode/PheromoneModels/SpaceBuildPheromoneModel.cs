using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.PheromoneModels
{
    public class SpaceBuildPheromoneModel : IPheromoneModel
    {
        private List<Pheromone> _initialPheromones;
        private List<Pheromone> _buildPheromones;
        private List<Pheromone> _spacePheromones;
        private double _initialPheromoneIntensity;
        private double _buildPheromoneIntensity;
        private double _spacePheromoneIntensity;

        /// <summary>
        /// Evaluates if an agent should build a cube according to its pheromone influences.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        /// <param name="agent">The agent who asks to build a cube or not.</param>
        /// <returns><value>true</value> if the agent should place a cube, otherwise <value>false</value>.</returns>
        public bool ShouldBuild(GlobalKnowledge globalKnowledge, Agent agent)
        {
            // Only build in spezific range around a SpacePheromone
            CalculatePheromoneInfluences(globalKnowledge, agent);
            return (_buildPheromoneIntensity >= 0.05 
                || Math.Abs(_buildPheromoneIntensity) < 0.0005) 
                && _spacePheromoneIntensity < 0.1 
                && _spacePheromoneIntensity > 0.02
                && _initialPheromoneIntensity < 0;
        }

        /// <summary>
        /// Evaluates if an agent should remove a cube according to its pheromone influences.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        /// <param name="agent">The agent who asks to remove a cube or not.</param>
        /// <returns><value>true</value> if the agent should remove a cube, otherwise <value>false</value>.</returns>
        public bool ShouldRemove(GlobalKnowledge globalKnowledge, Agent agent)
        {
            CalculatePheromoneInfluences(globalKnowledge, agent);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates if an agent should place a SpacePheromone according to its pheromone influences.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        /// <param name="agent">The agent who asks to place a SpacePheromone or not.</param>
        /// <returns><value>true</value> if the agent should place a SpacePheromone, otherwise <value>false</value>.</returns>
        public bool ShouldPlaceSpacePheromone(GlobalKnowledge globalKnowledge, Agent agent)
        {
            
            CalculatePheromoneInfluences(globalKnowledge, agent);
            Random random = new Random();
            if (random.NextDouble() < 0.2)
            {
                return _spacePheromoneIntensity < 0.1
                && _spacePheromoneIntensity > 0.05
                && _initialPheromoneIntensity < 0;
            }
            return false;
        }

        /// <summary>
        /// Updates the private pheromone lists.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        private void UpdatePheromones(GlobalKnowledge globalKnowledge)
        {
            _initialPheromones = globalKnowledge.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Initial).ToList();
            _buildPheromones = globalKnowledge.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Build).ToList();
            _spacePheromones = globalKnowledge.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Initial).ToList();
        }

        /// <summary>
        /// Updates the pheromone situation and calculates the influences of all PheromoneTypes on the agent at its current position.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        /// <param name="agent">The agent who asks for intensities.</param>
        private void CalculatePheromoneInfluences(GlobalKnowledge globalKnowledge, Agent agent)
        {
            // Update pheromone lists to current pheromone situation
            UpdatePheromones(globalKnowledge);

            // Calculate pheromone influences on specific agent
            AntBuildCalculations buildCalculations = new AntBuildCalculations();
            _initialPheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _initialPheromones);
            _buildPheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _buildPheromones);
            _spacePheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _spacePheromones);
        }
    }
}
