﻿using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.PheromoneModels
{
   /// <summary>
   /// Class SpaceBuildPheromoneModel.
   /// </summary>
   /// <seealso cref="SelfConstruction.AgentCode.Interfaces.IPheromoneModel" />
   public class SpaceBuildPheromoneModel : IPheromoneModel
    {
        private List<Pheromone> _initialPheromones = new List<Pheromone> {GlobalKnowledge.Instance.InitialPheromone};
        private List<Pheromone> _buildPheromones;
        private List<Pheromone> _spacePheromones;
        private double _initialPheromoneIntensity;
        private double _buildPheromoneIntensity;
        private double _spacePheromoneIntensity;
        private static int MAX_SPACEPHEROMONES = 2;

        /// <summary>
        /// Evaluates if an agent should build a cube according to its pheromone influences.
        /// </summary>
        /// <param name="agent">The agent who asks to build a cube or not.</param>
        /// <returns><value>true</value> if the agent should place a cube, otherwise <value>false</value>.</returns>
        public bool ShouldBuild(Position position)
        {
            // Only build in specific range around a SpacePheromone
            CalculatePheromoneInfluences(position);
            return /*(_buildPheromoneIntensity >= 0.0005 || Math.Abs(_buildPheromoneIntensity) < 0.0005) 
                &&*/ _spacePheromoneIntensity < 0.134
                && _spacePheromoneIntensity > 0.07
                && _initialPheromoneIntensity > 0;
        }

        /// <summary>
        /// Evaluates if an agent should remove a cube according to its pheromone influences.
        /// </summary>
        /// <param name="cubePosition">The cube position.</param>
        /// <returns><value>true</value> if the agent should remove a cube, otherwise <value>false</value>.</returns>
        public bool ShouldRemove(Position cubePosition)
        {
            CalculatePheromoneInfluences(cubePosition);
            if (!ShouldBuild(cubePosition))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates if an agent should place a SpacePheromone according to its pheromone influences.
        /// </summary>
        /// <param name="agent">The agent who asks to place a SpacePheromone or not.</param>
        /// <returns><value>true</value> if the agent should place a SpacePheromone, otherwise <value>false</value>.</returns>
        public bool ShouldPlaceSpacePheromone(Agent agent)
        {
            CalculatePheromoneInfluences(agent.Position);
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
            _buildPheromones = GlobalKnowledge.Instance.BuildPheromones.ToList();
            _spacePheromones = GlobalKnowledge.Instance.SpacePheromones.ToList();
        }

        /// <summary>
        /// Updates the pheromone situation and calculates the influences of all PheromoneTypes on the agent at its current position.
        /// </summary>
        /// <param name="agent">The agent who asks for intensities.</param>
        private void CalculatePheromoneInfluences(Position position)
        {
            // Update pheromone lists to current pheromone situation
            UpdatePheromones();

            // Calculate pheromone influences on specific agent
            AntBuildCalculations buildCalculations = new AntBuildCalculations();
            _initialPheromoneIntensity = buildCalculations.GetMostInfluentialIntesity(position, Pheromonetype.Initial);
            _buildPheromoneIntensity = buildCalculations.GetMostInfluentialIntesity(position, Pheromonetype.Build);
            _spacePheromoneIntensity = buildCalculations.GetMostInfluentialIntesity(position, Pheromonetype.Space);
        }
    }
}
