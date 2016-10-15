using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.MovementAlgorithm;
using SelfConstruction.AgentCode.PheromoneModels;

namespace SelfConstruction.AgentCode
{
   /// <summary>
   /// Class Agent.
   /// </summary>
   public class Agent
   {
      /// <summary>
      /// The position of the Agent. 
      /// </summary>
      public Position Position { get; set; }
      private readonly Movement _movement = new Movement();
      private double _lastBuildPheromoneInfluence = 0;

      /// <summary>
      /// Initializes a new instance of the <see cref="Agent"/> class.
      /// </summary>
      /// <param name="position">The position.</param>
      public Agent(Position? position = null)
      {
         Position = position ?? new Position(0, 0, 0);
      }

      /// <summary>
      /// Does the step of the agent.
      /// </summary>
      public void DoStep()
      {
         Remove();
         Build();
         PlaceSpacePheromone();
         MoveAgent();
         EvaporatePheromones();
      }

      private static void EvaporatePheromones()
      {
         // Temporary Enumerable used to calculate evaporation
         ConcurrentBag<Pheromone> build = new ConcurrentBag<Pheromone>();

         // Calculate evaporation
         foreach (Pheromone pheromone in GlobalKnowledge.Instance.BuildPheromones)
         {
            Pheromone t = pheromone;
            t.Intensity -= t.Intensity*t.VaporationRate;
            build.Add(t);
         }
         GlobalKnowledge.Instance.BuildPheromones = build;
      }

      private void MoveAgent()
      {
         // Update Pheromone intensities in GlobalKnowledge
         _lastBuildPheromoneInfluence = CalculateBuildingPheromoneInfluence();
         object currentMovementAction = _movement.ChooseMovementAction(this.Position);

         if (currentMovementAction is Position)
         {
            Move((Position) currentMovementAction);
         }
         else
         {
            Move((MovementAction) currentMovementAction);
         }
      }

      /// <summary>
      /// Evaluates if Agent should build a cube based to the current pheromone situation,
      /// and acts according to the result. 
      /// </summary>
      private void Build()
      {
         IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

         if (pheromoneModel.ShouldBuild(Position))
         {
            GlobalKnowledge.Instance.Blocks.Add(new BuildingShape(Position));
            GlobalKnowledge.Instance.StepBlocks.Add(new BuildingShape(Position));
            GlobalKnowledge.Instance.BuildPheromones.Add(new Pheromone(position: Position, intensity: 3,
               pheromonetype: Pheromonetype.Build, vaporationRate: 0.001));
            _movement.ResetCauseOfBuild();
         }
      }

      /// <summary>
      /// Evaluates if Agent should remove a cube based to the current pheromone situation,
      /// and acts according to the result. 
      /// </summary>
      private void Remove()
      {
         IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

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
            if (!GlobalKnowledge.Instance.Blocks.TryTake(out shapeToDelete))
            {
               Console.WriteLine(String.Format("Shape {0} could not be deleted!"), shapeToDelete);
            }
            IEnumerable<Pheromone> pheromonesToDelete =
               GlobalKnowledge.Instance.BuildPheromones.Where(
                  p => p.Position.Equals(shapeToDelete.Position));
            if (pheromonesToDelete.Any())
            {
               Pheromone pheromoneToDelete = pheromonesToDelete.FirstOrDefault();
               GlobalKnowledge.Instance.BuildPheromones.TryTake(out pheromoneToDelete);
            }
         }
      }

      /// <summary>
      /// Places the space pheromone.
      /// </summary>
      private void PlaceSpacePheromone()
      {
         IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();

         if (pheromoneModel.ShouldPlaceSpacePheromone(this))
         {
            GlobalKnowledge.Instance.StepBlocks.Add(new BuildingShape(Position));
            GlobalKnowledge.Instance.SpacePheromones.Add(new Pheromone(position: Position, intensity: 7.5,
               pheromonetype: Pheromonetype.Space, vaporationRate: 0));
            GlobalKnowledge.Instance.SpacePheromoneCounter++;
         }
      }

      private void Move(MovementAction action)
      {
         int random = new Random().Next(8);
         List<int> cubePositionList;

         switch (action)
         {
            case MovementAction.MoveLeft:
               // Negative movement action on X-Axis
               cubePositionList = new List<int> {2, 5, 8, 12, 15, 18, 22, 25, 28};
               break;
            case MovementAction.MoveRight:
               // Positive movement action on X-Axis
               cubePositionList = new List<int> {0, 3, 6, 10, 13, 16, 20, 23, 26};
               break;
            case MovementAction.MoveUp:
               // Positive movement action on Y-Axis
               cubePositionList = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8};
               break;
            case MovementAction.MoveDown:
               // Positive movement action on Y-Axis
               cubePositionList = new List<int> {20, 21, 22, 23, 24, 25, 26, 27, 28};
               break;
            case MovementAction.MoveBackward:
               // Positive movement action on Z-Axis
               cubePositionList = new List<int> {0, 1, 2, 10, 11, 12, 20, 21, 22};
               break;
            case MovementAction.MoveForward:
               // Positive movement action on Z-Axis
               cubePositionList = new List<int> {6, 7, 8, 16, 17, 18, 26, 27, 28};
               break;
            default:
               random = 0;
               cubePositionList = new List<int> {new Random().Next(28)};
               break;
         }

         Position newPosition = CalculateCartesianPosition(cubePositionList.ElementAt(random));

         if (Utils.Instance.IsPositionFree(newPosition))
         {
            // Write move action to log file
            double deltaX = this.Position.X - newPosition.X;
            double deltaY = this.Position.Y - newPosition.Y;
            double deltaZ = this.Position.Z - newPosition.Z;

            Position = newPosition;
         }
      }

      /// <summary>
      /// Moves to specified position.
      /// </summary>
      /// <param name="position">The position.</param>
      public void Move(Position position)
      {
         this.Position = position;
      }

      /// <summary>
      /// Gets the reward.
      /// </summary>
      /// <returns></returns>
      public double GetReward()
      {
         return CalculateBuildingPheromoneInfluence() - _lastBuildPheromoneInfluence;
      }


      /// <summary>
      /// Calculates the building pheromone influence.
      /// </summary>
      /// <returns></returns>
      private double CalculateBuildingPheromoneInfluence()
      {
         AntBuildCalculations antBuildCalculations = new AntBuildCalculations();
         return antBuildCalculations.SumUpPheromoneIntensity(Position,
            GlobalKnowledge.Instance.BuildPheromones.ToList());
      }

      /// <summary>
      /// Gets the surrounding cells.
      /// </summary>
      /// <returns></returns>
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

      /// <summary>
      /// Calculates the Cartesian position.
      /// </summary>
      /// <param name="cubePosition">The cube position.</param>
      /// <returns></returns>
      private Position CalculateCartesianPosition(int cubePosition)
      {
         Position nextPosition = this.Position;

         // Calculate Position X
         if (new List<int> {0, 3, 6, 10, 13, 16, 20, 23, 26}.Contains(cubePosition))
         {
            nextPosition.X += 1;
         }
         else if (new List<int> {2, 5, 8, 12, 15, 18, 22, 25, 28}.Contains(cubePosition))
         {
            nextPosition.X -= 1;
         }

         // Calculate Position Y
         if (new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8}.Contains(cubePosition))
         {
            nextPosition.Y += 1;
         }
         else if (new List<int> {20, 21, 22, 23, 24, 25, 26, 27, 28}.Contains(cubePosition))
         {
            nextPosition.Y -= 1;
         }

         // Calculate Position Z
         if (new List<int> {0, 1, 2, 10, 11, 12, 20, 21, 22}.Contains(cubePosition))
         {
            nextPosition.Z += 1;
         }
         else if (new List<int> {6, 7, 8, 16, 17, 18, 26, 27, 28}.Contains(cubePosition))
         {
            nextPosition.Z -= 1;
         }
         return nextPosition;
      }
   }
}