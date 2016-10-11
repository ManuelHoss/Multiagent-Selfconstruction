using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.RevitCode;
using Point = System.Drawing.Point;
using Utils = SelfConstruction.AgentCode.Utils;

namespace SelfConstruction
{
    [Transaction(TransactionMode.Manual)]
    public class SelfConstruction : IExternalCommand
    {
        private readonly Cube _cube = new Cube();
        private readonly Sphere _sphere = new Sphere();

        private List<ElementId> _tempAgents;

        public Result Execute(ExternalCommandData revit, ref string message, ElementSet elements)
        {
            for (int i = 0; i < 2000; i++)
            {
                revit.Application.OpenAndActivateDocument("C:\\multiagent-selfconstruction\\CSharp\\SelfConstruction\\SelfConstructionVorlage.rvt");
                Document doc = revit.Application.ActiveUIDocument.Document;
                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("Build and Calculate");
                    StartAgentsAndBuildBlocks(doc);
                    doc.Regenerate();
                    EnergyAnalysis.Instance.CalculateAndDisplayVolumeAndArea(doc);
                    transaction.Commit();
                }

                revit.Application.ActiveUIDocument.RefreshActiveView();
                
                CreateScreenshot(i);

                double[] areaAndVolume = EnergyAnalysis.Instance.GetAreaAndVolume(doc);
            }

            return Result.Succeeded;
        }

        private static void CreateScreenshot(int i)
        {
            int screenWidth = Screen.GetBounds(new Point(0, 0)).Width;
            int screenHeight = Screen.GetBounds(new Point(0, 0)).Height;
            Bitmap bmpScreenShot = new Bitmap(screenWidth, screenHeight);
            Graphics gfx = Graphics.FromImage(bmpScreenShot);
            gfx.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight - 50));
            if (!Directory.Exists("C:\\multiagent-selfconstruction\\CSharp\\Images"))
            {
                Directory.CreateDirectory("C:\\multiagent-selfconstruction\\CSharp\\Images");
            }
            bmpScreenShot.Save("C:\\multiagent-selfconstruction\\CSharp\\Images\\Screenshot" + i + ".jpg", ImageFormat.Jpeg);
        }


        /// <summary>
        /// Starts the agents and build blocks.
        /// </summary>
        /// <param name="doc">The document.</param>
        public void StartAgentsAndBuildBlocks(Document doc)
        {
            // Add initial Pheromone
            if (GlobalKnowledge.Instance.SpacePheromones.IsEmpty)
            {
                GlobalKnowledge.Instance.SpacePheromones.Add(new Pheromone(7.5, 0, Pheromonetype.Space, new Position(0, 0, 0)));
            }

            RunAgents(75, 1);

            // Create building cubes
            foreach (BuildingShape buildingShape in GlobalKnowledge.Instance.StepBlocks)
            {
                _cube.CreateCube(doc, new XYZ(buildingShape.Position.X, buildingShape.Position.Y, buildingShape.Position.Z), false);
            }
            // Create pheromone spheres
            foreach (var pheromone in GlobalKnowledge.Instance.SpacePheromones)
            {
                if (pheromone.Pheromonetype != Pheromonetype.Initial && pheromone.Pheromonetype != Pheromonetype.Build)
                {
                    _sphere.CreateSphere(doc, new XYZ(pheromone.Position.X, pheromone.Position.Y, pheromone.Position.Z), pheromone.Intensity, pheromone.Pheromonetype);
                }
            }

            // Remove all Blocks from current Step
            GlobalKnowledge.Instance.StepBlocks = new ConcurrentBag<BuildingShape>();

            // Remove old Agents from Doc 
            if (_tempAgents != null)
            {
                doc.Delete(_tempAgents);
            }

            // Remove all agents from list
            _tempAgents = new List<ElementId>();

            // Create agent cubes
            foreach (Agent agent in GlobalKnowledge.Instance.Agents)
            {
                _tempAgents.Add(_cube.CreateCube(doc, new XYZ(agent.Position.X, agent.Position.Y, agent.Position.Z), true));
            }
        }

        /// <summary>
        /// Runs the agents.
        /// </summary>
        /// <param name="agentCount">The agent count.</param>
        /// <param name="loops">The loops.</param>
        public void RunAgents(int agentCount, int loops)
        {
            if (GlobalKnowledge.Instance.Agents.Count != agentCount)
            {
                for (int i = 0; i < agentCount; i++)
                {
                    GlobalKnowledge.Instance.Agents.Add(new Agent());
                }
            }

            for (int i = 0; i < loops; i++)
            {
                List<Thread> workerThreads = new List<Thread>();

                // GlobalKnowledge.Instance.Agents.ElementAt(0).DoStep();

                // Replace for debugging
                foreach (Agent agent in GlobalKnowledge.Instance.Agents)
                {
                    Thread thread = new Thread(delegate () { agent.DoStep(); });
                    thread.Start();
                    workerThreads.Add(thread);
                }

                foreach (Thread workerThread in workerThreads)
                {
                    workerThread.Join(50);
                }
                Console.WriteLine("Loop: " + i);

                Utils.Instance.RemoveVaporatedPheromones();
            }
        }
    }
}