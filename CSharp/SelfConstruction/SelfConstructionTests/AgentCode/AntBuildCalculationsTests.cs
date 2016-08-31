using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Interfaces;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.PheromoneModels;

namespace SelfConstructionTests.AgentCode
{
    [TestClass()]
    public class AntBuildCalculationsTests
    {

        [TestMethod()]
        public void CalculateNewPositionTest()
        {
            //Check all possibilities, pheromone.x is higher, Y is equal, z is lower.
            Position position1 = new Position(0, 0, 1);
            Position position2 = new Position(2, 0, 0);

            AntBuildCalculations antBuildCalculations = new AntBuildCalculations();
            
            Assert.AreEqual(new Position(1,0,0), antBuildCalculations.CalculateNewPosition(position1, position2));
        }

        [TestMethod()]
        public void GetSortedPheromoneInfluencesTest()
        {
            ConcurrentBag<Pheromone> pheromones = new ConcurrentBag<Pheromone>();
            Pheromone pheromone1 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(100, 100, 100));
            Pheromone pheromone2 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(99, 100, 100));
            Pheromone pheromone3 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(100, 80, 100));
            pheromones.Add(pheromone1);
            pheromones.Add(pheromone2);
            pheromones.Add(pheromone3);

            GlobalKnowledge.Instance.Pheromones = pheromones;
        
            AntBuildCalculations antBuildCalculations = new AntBuildCalculations();

            List<Pheromone> returnValue = antBuildCalculations.GetSortedPheromoneInfluences(new Position(0, 0, 0));

            Assert.AreEqual(pheromone3, returnValue.First(), "Das erste Element ist falsch, Sortierung prüfen" );
            Assert.AreEqual(pheromone1, returnValue.Last(), "Das letzte Element ist falsch, Sortierung prüfen" );
        }

        [TestMethod()]
        public void ShouldBuildTest()
        {
            ConcurrentBag<Pheromone> pheromones = new ConcurrentBag<Pheromone>();
            Pheromone pheromone1 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(1, 10, 10));
            Pheromone pheromone2 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(9, 10, 10));
            Pheromone pheromone3 = new Pheromone(pheromonetype: Pheromonetype.Build, position: new Position(12, 8, 12));

            Pheromone pheromone4 = new Pheromone(pheromonetype: Pheromonetype.Initial, position: new Position(100, 100, 160));


            pheromones.Add(pheromone1);
            pheromones.Add(pheromone2);
            pheromones.Add(pheromone3);
            pheromones.Add(pheromone4);

            IPheromoneModel pheromoneModel = new SpaceBuildPheromoneModel();
            GlobalKnowledge.Instance.Pheromones = pheromones;

            Assert.IsFalse(pheromoneModel.ShouldBuild(new Position(0, 0, 0)),
                "Pheromone stärkenberechnung oder Funktion ist fehlerhaft");

        }
    }
}