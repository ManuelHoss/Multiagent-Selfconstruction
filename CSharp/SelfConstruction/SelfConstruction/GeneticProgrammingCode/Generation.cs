using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfConstruction.GeneticProgrammingCode.Models
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
