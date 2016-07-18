using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class SelfConstruction : IExternalCommand
    {
        private readonly Cube _cube = new Cube();


        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            Document doc = revit.Application.ActiveUIDocument.Document;
            GlobalKnowledge globalKnowledge = new GlobalKnowledge
            {
                Agents = new ConcurrentBag<Agent>(),
                Blocks = new ConcurrentBag<BuildingShape>(),
                Pheromones = new ConcurrentBag<Pheromone>()
            };

            RunAgents(globalKnowledge, 10, 100);
            foreach (BuildingShape buildingShape in globalKnowledge.Blocks)
            {
                _cube.CreateCube(doc, new XYZ(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z));
            }
            //_cube.CreateCube(doc, new XYZ(3,4,6));
            return Result.Succeeded;
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