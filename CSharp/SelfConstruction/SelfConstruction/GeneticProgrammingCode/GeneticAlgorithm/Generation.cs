using System.Collections.Generic;

namespace SelfConstruction.GeneticProgrammingCode.GeneticAlgorithm
{
    public class Generation
    {
        public int GenerationId { get; private set; }
        public List<Individual> Population { get; private set; }
        public double FitnessValue { get; private set; }

        public Generation(int generationId)
        {
            GenerationId = generationId;
            Population = new List<Individual>();
            FitnessValue = 0;
        }

        /// <summary>
        /// Adds a individual to the Population
        /// </summary>
        /// <param name="individual">Individual that should be added.</param>
        public void AddIndividual(Individual individual)
        {
            Population.Add(individual);
            UpdateFitnessValue();
        }

        /// <summary>
        /// Sets the fitness value of the generation. (Average of the fitness of all individuals)
        /// </summary>
        public void UpdateFitnessValue()
        {
            double fitnessValue = 0;
            foreach (var individual in Population)
            {
                fitnessValue += individual.FitnessValue;
            }

            FitnessValue = fitnessValue/Population.Count;
        }
    }
}
