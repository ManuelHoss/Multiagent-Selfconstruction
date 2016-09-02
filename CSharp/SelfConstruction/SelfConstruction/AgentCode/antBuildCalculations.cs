using System;
using System.Collections.Concurrent;
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
        public List<Pheromone> GetSortedPheromoneInfluences(Position position)
        {
            List<Tuple<Pheromone, double>> mostInfluential = new List<Tuple<Pheromone, double>>();
           
            foreach (Pheromone pheromone in GlobalKnowledge.Instance.BuildPheromones)
            {
                double distance = Utils.Instance.CalculateDistanceToBrick(pheromone.Position, position);

                double pheromoneIntensity = pheromone.Intensity/(distance*distance);
                mostInfluential.Add(new Tuple<Pheromone, double>(pheromone, pheromoneIntensity));
            }

            return mostInfluential.OrderBy(pheromone => pheromone.Item2).Select(pheromone => pheromone.Item1).Reverse().ToList();
        }
        
        /// <summary>
        /// Sums up pheromone intensity.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="pheromones">The pheromones.</param>
        /// <returns>System.Double.</returns>
        public double SumUpPheromoneIntensity(Position position, List<Pheromone> pheromones)
        {
            double pheromoneIntensity = 0;

            foreach (Pheromone pheromone in pheromones)
            {
                double distance = Utils.Instance.CalculateDistanceToBrick(pheromone.Position, position);

                pheromoneIntensity += pheromone.Intensity/(distance*distance);
            }
            return pheromoneIntensity;
        }

        public double GetMostInfluentialIntesity(Position position, Pheromonetype pheromonetype)
        {
            ConcurrentBag<Pheromone> list = pheromonetype == Pheromonetype.Build ? GlobalKnowledge.Instance.BuildPheromones :
            GlobalKnowledge.Instance.SpacePheromones;
            List<Pheromone> pheromones = list.Where(p => p.Pheromonetype == pheromonetype).ToList();
            double maxIntesity = 0;
            if (pheromonetype != Pheromonetype.Initial)
            {
                foreach (Pheromone pheromone in pheromones)
                {
                    double distance = Utils.Instance.CalculateDistanceToBrick(pheromone.Position, position);
                    double pheromoneIntensity = pheromone.Intensity / (distance * distance);
                    if (maxIntesity < pheromoneIntensity)
                    {
                        maxIntesity = pheromoneIntensity;
                    }
                }
            }
            else
            {
                double distance = Utils.Instance.CalculateDistanceToBrick(GlobalKnowledge.Instance.InitialPheromone.Position, position);
                maxIntesity = GlobalKnowledge.Instance.InitialPheromone.Intensity / (distance * distance);
            }
            return maxIntesity;
        }
    }
}
