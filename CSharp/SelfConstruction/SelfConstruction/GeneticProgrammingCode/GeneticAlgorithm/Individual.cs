using System.Collections.Generic;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;

namespace SelfConstruction.GeneticProgrammingCode.GeneticAlgorithm
{
    public abstract class Individual
    {
        public int IndividualId { get; private set; }
        public int GenerationId { get; private set; }
        public List<List<Action>> Instructions { get; private set; }
        public double FitnessValue { get; private set; }

        public Individual(int individualId, int generationId, List<List<Action>> instructions, double fitnessValue)
        {
            IndividualId = individualId;
            GenerationId = generationId;
            Instructions = instructions;
            FitnessValue = fitnessValue;
        }
    }
}
