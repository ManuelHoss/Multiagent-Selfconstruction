using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;
using Autodesk.Revit.UI.Selection;
using SelfConstruction.RevitCode;

namespace SelfConstruction
{
    [Transaction(TransactionMode.Automatic)]
    public class SelfConstruction : IExternalCommand
    {
        private readonly Cube _cube = new Cube();
        private readonly Sphere _sphere = new Sphere();

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            Document doc = revit.Application.ActiveUIDocument.Document;
            StartAgentsAndBuildBlocks(doc);
            doc.Regenerate();
            EnergyAnalysis.Instance.CalculateAndDisplayVolumeAndArea(doc);
//            EnergyAnalysis(doc);
            return Result.Succeeded;
        }

        private void StartAgentsAndBuildBlocks(Document doc)
        {
            GlobalKnowledge globalKnowledge = new GlobalKnowledge
            {
                Agents = new ConcurrentBag<Agent>(),
                Blocks = new ConcurrentBag<BuildingShape>(),
                Pheromones = new ConcurrentBag<Pheromone>()
            };

            // Add initial Pheromone
            globalKnowledge.Pheromones.Add(new Pheromone(5, 0, Pheromonetype.Initial, new Position(0,0,0)));
            // Display Radius of InitialPheromone
            _sphere.CreateSphere(doc, new XYZ(0, 0, 0), globalKnowledge.Pheromones.FirstOrDefault(p => p.Pheromonetype == Pheromonetype.Initial).Intensity, Pheromonetype.Initial);

            RunAgents(globalKnowledge, 250, 150);
            // Create building cubes
            foreach (BuildingShape buildingShape in globalKnowledge.Blocks)
            {
                Categories allCategories = doc.Settings.Categories;
                _cube.CreateCube(doc, new XYZ(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z), false);
                globalKnowledge.Pheromones.Add(new Pheromone(10, 0.00001, Pheromonetype.Build, new Position(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z)));
            }

            // Create agent cubes
            foreach (Agent agent in globalKnowledge.Agents)
            {
                Categories allCategories = doc.Settings.Categories;
                _cube.CreateCube(doc, new XYZ(agent.Position.X, agent.Position.Y, agent.Position.Z), true);
            }
        }

        public void RunAgents(GlobalKnowledge globalKnowledge, int agentCount, int loops)
        {
            for (int i = 0; i < agentCount; i++)
            {
                globalKnowledge.Agents.Add(new Agent());
            }

            for (int i = 0; i < loops; i++)
            {
                List<Thread> workerThreads = new List<Thread>();

                foreach (Agent agent in globalKnowledge.Agents)
                {
                    Thread thread = new Thread(delegate () { agent.DoStep(globalKnowledge); });
                    thread.Start();
                    workerThreads.Add(thread);
                }

                foreach (Thread workerThread in workerThreads)
                {
                    workerThread.Join(50);
                }
                Console.WriteLine("Loop: " + i);

                Utils.Instance.RemoveVaporatedPheromones(globalKnowledge);
            }
        }


    }
}