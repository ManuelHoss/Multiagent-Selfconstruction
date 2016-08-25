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
        private static int MAX_SPACEPHEROMONES = 1;

        /// <summary>
        /// Evaluates if an agent should build a cube according to its pheromone influences.
        /// </summary>
        /// <param name="agent">The agent who asks to build a cube or not.</param>
        /// <returns><value>true</value> if the agent should place a cube, otherwise <value>false</value>.</returns>
        public bool ShouldBuild(Agent agent)
        {
            // Only build in spezific range around a SpacePheromone
            CalculatePheromoneInfluences(agent);
            return /*(_buildPheromoneIntensity >= 0.0005 || Math.Abs(_buildPheromoneIntensity) < 0.0005) 
                &&*/ _spacePheromoneIntensity < 0.15
                && _spacePheromoneIntensity > 0.13
                && _initialPheromoneIntensity > 0;
        }

        /// <summary>
        /// Evaluates if an agent should remove a cube according to its pheromone influences.
        /// </summary>
        /// <param name="agent">The agent who asks to remove a cube or not.</param>
        /// <returns><value>true</value> if the agent should remove a cube, otherwise <value>false</value>.</returns>
        public bool ShouldRemove(Agent agent)
        {
            CalculatePheromoneInfluences(agent);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Evaluates if an agent should place a SpacePheromone according to its pheromone influences.
        /// </summary>
        /// <param name="globalKnowledge">Knowledge base which holds information about every
        /// agent, pheromone and building cube.</param>
        /// <param name="agent">The agent who asks to place a SpacePheromone or not.</param>
        /// <returns><value>true</value> if the agent should place a SpacePheromone, otherwise <value>false</value>.</returns>
        public bool ShouldPlaceSpacePheromone(Agent agent)
        {
            CalculatePheromoneInfluences(agent);
            Random random = new Random();
            if (random.NextDouble() < 0.01 && GlobalKnowledge.Instance.SpacePheromoneCounter <= MAX_SPACEPHEROMONES)
            {
                return _spacePheromoneIntensity < 0.1
                && _spacePheromoneIntensity > 0.08
                && _initialPheromoneIntensity > 0;
            }
            return false;
        }

        /// <summary>
        /// Updates the private pheromone lists.
        /// </summary>
        private void UpdatePheromones()
        {
            _initialPheromones = GlobalKnowledge.Instance.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Initial).ToList();
            _buildPheromones = GlobalKnowledge.Instance.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Build).ToList();
            _spacePheromones = GlobalKnowledge.Instance.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Space).ToList();
        }

        /// <summary>
        /// Updates the pheromone situation and calculates the influences of all PheromoneTypes on the agent at its current position.
        /// </summary>
        /// <param name="agent">The agent who asks for intensities.</param>
        private void CalculatePheromoneInfluences(Agent agent)
        {
            // Update pheromone lists to current pheromone situation
            UpdatePheromones();

            // Calculate pheromone influences on specific agent
            AntBuildCalculations buildCalculations = new AntBuildCalculations();
            _initialPheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _initialPheromones);
            _buildPheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _buildPheromones);
            _spacePheromoneIntensity = buildCalculations.SumUpPheromoneIntensity(agent, _spacePheromones);
        }
    }
}
