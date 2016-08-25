using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;

namespace SelfConstructionTests.AgentCode
{
    [TestClass()]
    public class UtilsTests
    {
        [TestMethod()]
        public void CalculateDistanceToBrickTest()
        {
            Position position1 = new Position(0, 0, 1);
            Position position2 = new Position(2, 0, 1);

            Assert.AreEqual(2, Utils.Instance.CalculateDistanceToBrick(position1, position2), "Unequal");
        }

        [TestMethod()]
        public void IsPositionFreeTest()
        {
            Position position1 = new Position(0, 0, 1);
            Position position2 = new Position(2, 0, 1);

            Agent agent = new Agent(new Position(0,0,0));
            Agent agent1 = new Agent(new Position(0,0,1));
            BuildingShape buildingShape = new BuildingShape(new Position(0,0,0));
            BuildingShape buildingshape1 = new BuildingShape(new Position(1,0,1));

            ConcurrentBag<Agent> agents = new ConcurrentBag<Agent> {agent, agent1};
            ConcurrentBag<BuildingShape> blocks = new ConcurrentBag<BuildingShape> {buildingShape, buildingshape1};
            
            GlobalKnowledge.Instance.Agents = agents;
            GlobalKnowledge.Instance.Blocks = blocks;

            Assert.IsFalse(Utils.Instance.IsPositionFree(position1));
            Assert.IsTrue(Utils.Instance.IsPositionFree(position2));
            
        }

        [TestMethod()]
        public void IsPositionEqualTest()
        {
            Position position1 = new Position(0,0,1);
            Position position2 = new Position(0,0,1);
            Position position3 = new Position(2,0,1);


            Assert.IsTrue(Utils.Instance.IsPositionEqual(position1, position2), "Gleiche Positionen sind als ungleich erkannt worden");
            Assert.IsFalse(Utils.Instance.IsPositionEqual(position1, position3), "Ungleiche Positionen sind als gleich erkannt worden");

        }
    }
}