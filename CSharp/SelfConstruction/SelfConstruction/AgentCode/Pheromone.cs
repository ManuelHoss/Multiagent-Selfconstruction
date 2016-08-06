using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode
{
    public struct Pheromone
    {
        public double Intensity;
        public double VaporationRate;
        public Pheromonetype Pheromonetype;
        public Position? Position;

        public Pheromone(double intensity = 50, double vaporationRate = 0.0000001, 
            Pheromonetype pheromonetype = Pheromonetype.Neutral, Position? position = null)
        {
            Intensity = intensity;
            VaporationRate = vaporationRate;
            Pheromonetype = pheromonetype;
            Position = position;
        }
    }
}
