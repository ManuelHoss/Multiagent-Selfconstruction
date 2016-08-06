using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
    public class AntBuildCalculations
    {
        /// <summary>
        /// Calculates the new position of an Agent.
        /// </summary>
        /// <param name="agentPosition">The agent position.</param>
        /// <param name="pheromone">The pheromone.</param>
        /// <returns>Position.</returns>
        public Position CalculateNewPosition(Position agentPosition, Position pheromone)
        {
            agentPosition.X = Math.Abs(agentPosition.X - pheromone.X) > 0.01 ? 
                agentPosition.X < pheromone.X ?
                agentPosition.X + 1 : agentPosition.X - 1 : 0;
            agentPosition.Y = Math.Abs(agentPosition.Y - pheromone.Y) > 0.01 ?
                agentPosition.Y < pheromone.Y ?
                agentPosition.Y + 1 : agentPosition.Y - 1 : 0;
            agentPosition.Z = Math.Abs(agentPosition.Z - pheromone.Z) > 0.01 ?
                agentPosition.Z < pheromone.Z ?
                agentPosition.Z + 1 : agentPosition.Z - 1 : 0;

            return agentPosition;
        }

        /// <summary>
        /// Gets the sorted pheromone influences.
        /// </summary>
        /// <param name="pheromones">The pheromones.</param>
        /// <param name="agent">The agent.</param>
        /// <returns>List&lt;Pheromone&gt;.</returns>
        public List<Pheromone> GetSortedPheromoneInfluences(GlobalKnowledge globalKnowledge, Agent agent)
        {
            List<Tuple<Pheromone, double>> mostInfluential = new List<Tuple<Pheromone, double>>();
           
            foreach (Pheromone pheromone in globalKnowledge.Pheromones)
            {
                double distance = Utils.Instance.CalculateDistanceToBrick(pheromone.Position, agent.Position);

                double pheromoneIntensity = pheromone.Intensity/(distance*distance);
                mostInfluential.Add(new Tuple<Pheromone, double>(pheromone, pheromoneIntensity));
            }

            return mostInfluential.OrderBy(pheromone => pheromone.Item2).Select(pheromone => pheromone.Item1).Reverse().ToList();
        }

        /// <summary>
        /// Checks whether the Agent should build or not
        /// </summary>
        /// <param name="pheromones">The pheromones.</param>
        /// <param name="agent">The agent.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ShouldBuild(GlobalKnowledge globalKnowledge, Agent agent)
        {
            
            List<Pheromone> buildPheromones = globalKnowledge.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Build).ToList();
            List<Pheromone> initialPheromones = globalKnowledge.Pheromones.Where(pheromone => pheromone.Pheromonetype == Pheromonetype.Initial).ToList();

            double buildPheromoneIntensity = SumUpPheromoneIntensity(agent, buildPheromones);
            double initialPheromoneIntensity = SumUpPheromoneIntensity(agent, initialPheromones);

            return (buildPheromoneIntensity >= 0.5 || Math.Abs(buildPheromoneIntensity) < 0.0005) && initialPheromoneIntensity < 0.05;
        }

        /// <summary>
        /// Sums up pheromone intensity.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="pheromones">The pheromones.</param>
        /// <returns>System.Double.</returns>
        private double SumUpPheromoneIntensity(Agent agent, List<Pheromone> pheromones)
        {
            double initialPheromoneIntensity = 0;

            foreach (Pheromone initialPheromone in pheromones)
            {
                double distance = Utils.Instance.CalculateDistanceToBrick(initialPheromone.Position, agent.Position);

                initialPheromoneIntensity += initialPheromone.Intensity/(distance*distance);
            }
            return initialPheromoneIntensity;
        }
    }
}
