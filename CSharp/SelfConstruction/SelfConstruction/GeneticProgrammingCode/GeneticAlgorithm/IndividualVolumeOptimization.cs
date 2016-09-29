using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfConstruction.GeneticProgrammingCode.Models;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;

namespace SelfConstruction.GeneticProgrammingCode.GeneticAlgorithm
{
    public class IndividualVolumeOptimization : Individual
    {
        public double IndividualVolume { get; set; }

        public IndividualVolumeOptimization(int individualId, int generationId, List<List<Action>> instructions, double fitnessValue, double individualVolume) : base(individualId, generationId, instructions, fitnessValue)
        {
            IndividualVolume = individualVolume;
        }
    }
}
