using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.PheromoneModels;

namespace SelfConstruction.AgentCode
{
    public class Agent
    {
        public Position Position;
        public BuildingShape? Payload;
        public String logString = "";

        public Agent(Position? position = null, BuildingShape? payload = null)
        {
            Position = position ?? new Position(0, 0, 0);
            Payload = payload;
        }

        public void DoStep(GlobalKnowledge globalKnowledge)
        {
            Build(globalKnowledge);
            PlaceSpacePheromone(globalKnowledge);
            MoveRandom(globalKnowledge);

            foreach (Pheromone t in globalKnowledge.Pheromones)
            {
                Pheromone pheromone = t;
                pheromone.Intensity -= pheromone.Intensity*pheromone.VaporationRate;
            }
        }

        private void Build(GlobalKnowledge globalKnowledge)
        {
            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

            if (pheromoneModel.ShouldBuild(globalKnowledge, this))
            {
                globalKnowledge.Blocks.Add(new BuildingShape(Position));
                globalKnowledge.StepBlocks.Add(new BuildingShape(Position));
                globalKnowledge.Pheromones.Add(new Pheromone(position: Position, intensity: 3, pheromonetype: Pheromonetype.Build, vaporationRate: 0.001));
                // Write build action to log file
                logString += "BUILD|";
            }
        }

        private void PlaceSpacePheromone(GlobalKnowledge globalKnowledge)
        {
            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

            if (pheromoneModel.ShouldPlaceSpacePheromone(globalKnowledge, this))
            {
                globalKnowledge.StepBlocks.Add(new BuildingShape(Position));
                globalKnowledge.Pheromones.Add(new Pheromone(position: Position, intensity: 7.5, pheromonetype: Pheromonetype.Space, vaporationRate: 0));
                globalKnowledge.SpacePheromoneCounter++;
                // Write SpacePheromone action to log file
                logString += "SPACE|";
            }
        }

        private void MoveRandom(GlobalKnowledge globalKnowledge)
        {
            List<Position> surroundingCells = GetSurroundingCells();
            
            for (int i = 0; i < 28; i++)
            {
                int random = new Random().Next(0, 27);

                if (Utils.Instance.IsPositionFree(globalKnowledge, surroundingCells[random]))
                {
                    // Write move action to log file
                    double deltaX = this.Position.X - surroundingCells[random].X;
                    double deltaY = this.Position.Y - surroundingCells[random].Y;
                    double deltaZ = this.Position.Z - surroundingCells[random].Z;
                    logString += String.Format("MOVE({0},{1},{2})|", deltaX, deltaY, deltaZ);

                    // Update Position
                    Position = surroundingCells[random];
                    return;
                }
            }
        }

        private List<Position> GetSurroundingCells()
        {
            List<Position> surroundingCartesianCoordinates = new List<Position>();

            for (int i = 0; i < 29; i++)
            {
                if (i != 9 && i != 19)
                {
                    surroundingCartesianCoordinates.Add(CalculateNextPosition(i));
                }
            }
            return surroundingCartesianCoordinates;
        }

        private Position CalculateNextPosition(int cubePosition)
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
            if (0<cubePosition && cubePosition<8)
            {
                nextPosition.Y += 1;
            }
            else if (20<cubePosition && cubePosition<28)
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
