using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning;
using SelfConstruction.AgentCode.PheromoneModels;

namespace SelfConstruction.AgentCode
{
    public class Agent
    {
        public Position Position;
        public BuildingShape? Payload;
        public String logString = "";

        private double _lastBuildPheromoneInfluence = 0;

        public Agent(Position? position = null, BuildingShape? payload = null)
        {
            Position = position ?? new Position(0, 0, 0);
            Payload = payload;
        }

        public void DoStep()
        {
            Remove();
            Build();
            PlaceSpacePheromone();
            
            MovementQLearning.Instance.ExecuteMovement(this, GetCurrentState());
            _lastBuildPheromoneInfluence = CalculateBuildingPheromoneInfluence();

            // Vaporation process
            foreach (Pheromone t in GlobalKnowledge.Instance.Pheromones)
            {
                Pheromone pheromone = t;
                pheromone.Intensity -= pheromone.Intensity*pheromone.VaporationRate;
            }
        }

        private void Build()
        {
            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

            if (pheromoneModel.ShouldBuild(Position))
            {
                GlobalKnowledge.Instance.Blocks.Add(new BuildingShape(Position));
                GlobalKnowledge.Instance.StepBlocks.Add(new BuildingShape(Position));
                GlobalKnowledge.Instance.Pheromones.Add(new Pheromone(position: Position, intensity: 3, pheromonetype: Pheromonetype.Build, vaporationRate: 0.001));
                // Write build action to log file
                logString += "BUILD|";
            }
        }

        private void Remove()
        {
            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();
            AntBuildCalculations antBuildCalculations = new AntBuildCalculations();

            // Check if there is a cube in V26 of the Agent that has to be removed
            List<Position> surroundingCells = GetSurroundingCells();
            List<BuildingShape> shapesToDelete = new List<BuildingShape>();
            foreach (var buildingShape in GlobalKnowledge.Instance.Blocks)
            {
                if (surroundingCells.Contains(buildingShape.Position))
                {
                    if (pheromoneModel.ShouldRemove(buildingShape.Position))
                    {
                        shapesToDelete.Add(buildingShape);
                    }
                }
            }

            // Remove if necessary
            for (int i = 0; i < shapesToDelete.Count; i++)
            {
                BuildingShape shapeToDelete = shapesToDelete[i];
                if(!GlobalKnowledge.Instance.Blocks.TryTake(out shapeToDelete))
                {
                    Console.WriteLine(String.Format("Shape {0} could not be deleted!"), shapeToDelete);
                }
                Pheromone pheromoneToDelete =
                    GlobalKnowledge.Instance.Pheromones.FirstOrDefault(
                        p => p.Position.Equals(shapeToDelete.Position) && p.Pheromonetype == Pheromonetype.Build);
                GlobalKnowledge.Instance.Pheromones.TryTake(out pheromoneToDelete);
            }
        }

        private void PlaceSpacePheromone()
        {
            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

            if (pheromoneModel.ShouldPlaceSpacePheromone(this))
            {
                GlobalKnowledge.Instance.StepBlocks.Add(new BuildingShape(Position));
                GlobalKnowledge.Instance.Pheromones.Add(new Pheromone(position: Position, intensity: 7.5, pheromonetype: Pheromonetype.Space, vaporationRate: 0));
                GlobalKnowledge.Instance.SpacePheromoneCounter++;
                // Write SpacePheromone action to log file
                logString += "SPACE|";
            }
        }

        public void Move(MovementAction action)
        {
            int random = new Random().Next(8);
            List<int> cubePositionList;

            switch (action)
            {
                case MovementAction.MoveLeft:
                    // Negative movent on X-Axis
                    cubePositionList = new List<int>() { 2, 5, 8, 12, 15, 18, 22, 25, 28 };
                    break;
                case MovementAction.MoveRight:
                    // Positive movent on X-Axis
                    cubePositionList = new List<int>() { 0, 3, 6, 10, 13, 16, 20, 23, 26 };
                    break;
                case MovementAction.MoveUp:
                    // Positive movent on Y-Axis
                    cubePositionList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
                    break;
                case MovementAction.MoveDown:
                    // Positive movent on Y-Axis
                    cubePositionList = new List<int>() { 20, 21, 22, 23, 24, 25, 26, 27, 28 };
                    break;
                case MovementAction.MoveBackward:
                    // Positive movent on Z-Axis
                    cubePositionList = new List<int>() { 0, 1, 2, 10, 11, 12, 20, 21, 22 };
                    break;
                case MovementAction.MoveForward:
                    // Positive movent on Z-Axis
                    cubePositionList = new List<int>() { 6, 7, 8, 16, 17, 18, 26, 27, 28 };
                    break;
                default:
                    random = 0;
                    cubePositionList = new List<int>() { new Random().Next(28) };
                    break;
            }
            
            Position newPosition = CalculateCartesianPosition(cubePositionList.ElementAt(random));

            if (Utils.Instance.IsPositionFree(newPosition))
            {
                // Write move action to log file
                double deltaX = this.Position.X - newPosition.X;
                double deltaY = this.Position.Y - newPosition.Y;
                double deltaZ = this.Position.Z - newPosition.Z;
                logString += String.Format("MOVE({0},{1},{2})|", deltaX, deltaY, deltaZ);

                Position = newPosition;
            }
        }

        public double GetReward()
        {
            return CalculateBuildingPheromoneInfluence() - _lastBuildPheromoneInfluence;
        }

        private State GetCurrentState()
        {
            if(MovementQLearning.Instance.QTable.Any(q => q.GetState().Position.Equals(Position)))
            {
                var state = MovementQLearning.Instance.QTable.FirstOrDefault(q => q.GetState().Position.Equals(Position));
                if (state != null)
                {
                    return state.GetState();
                }
            }
            return new State(Position);
        }

        private double CalculateBuildingPheromoneInfluence()
        {
            AntBuildCalculations antBuildCalculations = new AntBuildCalculations();
            return antBuildCalculations.SumUpPheromoneIntensity(Position,
                GlobalKnowledge.Instance.Pheromones.Where(p => p.Pheromonetype == Pheromonetype.Build).ToList());
        }

        private List<Position> GetSurroundingCells()
        {
            List<Position> surroundingCartesianCoordinates = new List<Position>();

            for (int i = 0; i < 29; i++)
            {
                if (i != 9 && i != 19)
                {
                    surroundingCartesianCoordinates.Add(CalculateCartesianPosition(i));
                }
            }
            return surroundingCartesianCoordinates;
        }

        private Position CalculateCartesianPosition(int cubePosition)
        {
            Position nextPosition = this.Position;

            // Calculate Position X
            if (new List<int> {0, 3, 6, 10, 13, 16, 20, 23, 26}.Contains(cubePosition))
            {
                nextPosition.X += 1;
            }
            else if (new List<int> { 2, 5, 8, 12, 15, 18, 22, 25, 28 }.Contains(cubePosition))
            {
                nextPosition.X -= 1;
            }

            // Calculate Position Y
            if (new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.Contains(cubePosition))
            {
                nextPosition.Y += 1;
            }
            else if (new List<int> { 20, 21, 22, 23, 24, 25, 26, 27, 28 }.Contains(cubePosition))
            {
                nextPosition.Y -= 1;
            }

            // Calculate Position Z
            if (new List<int> { 0, 1, 2, 10, 11, 12, 20, 21, 22 }.Contains(cubePosition))
            {
                nextPosition.Z += 1;
            }
            else if (new List<int> { 6, 7, 8, 16, 17, 18, 26, 27, 28 }.Contains(cubePosition))
            {
                nextPosition.Z -= 1;
            }
            return nextPosition;
        }
    }
}
