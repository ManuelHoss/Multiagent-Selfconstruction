using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction
{
    [Transaction(TransactionMode.Automatic)]
    public class SelfConstruction : IExternalCommand
    {
        private readonly Cube _cube = new Cube();
        
        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            UIApplication uiapp = revit.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = revit.Application.ActiveUIDocument.Document;
            StartAgentsAndBuildBlocks(doc, app);
            //EnergyAnalysis(doc);
            return Result.Succeeded;
        }
        private void EnergyAnalysis(Document doc)
        {
            EnergyAnalysisDetailModelOptions options = new EnergyAnalysisDetailModelOptions
            {
                Tier =EnergyAnalysisDetailModelTier.Final,
                EnergyModelType = EnergyModelType.SpatialElement
            };

            EnergyAnalysisDetailModel analysisDetailModel = EnergyAnalysisDetailModel.Create(doc, options);

            IList<EnergyAnalysisSpace> spaces = analysisDetailModel.GetAnalyticalSpaces();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Spaces: " + spaces.Count);
            foreach (EnergyAnalysisSpace space in spaces)
            {
                builder.AppendLine(space.Name + " InnVolume " + space.InnerVolume);
            }
            TaskDialog.Show("EAM", builder.ToString());
        }

        private void StartAgentsAndBuildBlocks(Document doc, Application app)
        {
            GlobalKnowledge globalKnowledge = new GlobalKnowledge
            {
                Agents = new ConcurrentBag<Agent>(),
                Blocks = new ConcurrentBag<BuildingShape>(),
                Pheromones = new ConcurrentBag<Pheromone>()
            };

            RunAgents(globalKnowledge, 10, 100);
            // Create building cubes
            foreach (BuildingShape buildingShape in globalKnowledge.Blocks)
            {
                Categories allCategories = doc.Settings.Categories;
                _cube.CreateCube(doc, new XYZ(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z), false);
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