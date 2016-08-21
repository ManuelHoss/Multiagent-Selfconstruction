using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.GeneticProgrammingCode;
using SelfConstruction.GeneticProgrammingCode.Logger;
using SelfConstruction.RevitCode;
using Utils = SelfConstruction.AgentCode.Utils;

namespace SelfConstruction
{
    [Transaction(TransactionMode.Manual)]
    public class SelfConstruction : IExternalCommand
    {
        private readonly Cube _cube = new Cube();
        private readonly Sphere _sphere = new Sphere();
        public GlobalKnowledge GlobalKnowledge { get; private set; }

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            for (int i = 0; i < 1; i++)
            {
                revit.Application.OpenAndActivateDocument(
                    "C:\\multiagent-selfconstruction\\CSharp\\SelfConstruction\\SelfConstructionVorlage.rvt");
                Document doc = revit.Application.ActiveUIDocument.Document;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Build and Calculate");
                    StartAgentsAndBuildBlocks(doc);
                    doc.Regenerate();
                    EnergyAnalysis.Instance.CalculateAndDisplayVolumeAndArea(doc);
                    transaction.Commit();
                }

                double[] areaAndVolume = EnergyAnalysis.Instance.GetAreaAndVolume(doc);

                //TODO: Here we should do the genetic programming and checking the volume
//                using (Transaction transaction = new Transaction(doc))
//                    {
//                        transaction.Start("Remove");
//                        FilteredElementCollector allDirectShapes =
//                        new FilteredElementCollector(doc).OfClass(typeof(DirectShape));
//                    doc.Delete(allDirectShapes.ToElementIds());
//                    transaction.Commit();
//                }
            }

            return Result.Succeeded;
        }


        public void StartAgentsAndBuildBlocks(Document doc)
        {
            GlobalKnowledge = new GlobalKnowledge
            {
                Agents = new ConcurrentBag<Agent>(),
                Blocks = new ConcurrentBag<BuildingShape>(),
                Pheromones = new ConcurrentBag<Pheromone>()
            };

            // Add initial Pheromone
            GlobalKnowledge.Pheromones.Add(new Pheromone(5, 0, Pheromonetype.Initial, new Position(0,0,0)));
            // Display Radius of InitialPheromone
            _sphere.CreateSphere(doc, new XYZ(0, 0, 0), GlobalKnowledge.Pheromones.FirstOrDefault(p => p.Pheromonetype == Pheromonetype.Initial).Intensity, Pheromonetype.Initial);

            RunAgents(GlobalKnowledge, 25, 150);
            // Create building cubes
            foreach (BuildingShape buildingShape in GlobalKnowledge.Blocks)
            {
                Categories allCategories = doc.Settings.Categories;
                _cube.CreateCube(doc, new XYZ(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z), false);
                GlobalKnowledge.Pheromones.Add(new Pheromone(10, 0.00001, Pheromonetype.Build, new Position(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z)));
            }

            // Create agent cubes
            foreach (Agent agent in GlobalKnowledge.Agents)
            {
                Categories allCategories = doc.Settings.Categories;
                _cube.CreateCube(doc, new XYZ(agent.Position.X, agent.Position.Y, agent.Position.Z), true);
            }

            // Write log file
            InstructionUtils instructionUtils = new InstructionUtils();
            instructionUtils.WriteActionSequenceToFile(GlobalKnowledge.Agents.ToList());
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