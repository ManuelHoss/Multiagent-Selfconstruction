using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;
using SelfConstruction.GeneticProgrammingCode.Logger;
using SelfConstruction.GeneticProgrammingCode.Models;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;
using Document = Autodesk.Revit.DB.Document;

namespace SelfConstruction.GeneticProgrammingCode.GeneticAlgorithm
{
    public abstract class GeneticAlgorithm
    {
        public Generation LastGeneration { get; set; }
        public Generation CurrentGeneration { get; set; }
        public int GenerationIdCounter { get; set; }
        public int IndividualIdCouter { get; set; }
        public int NumberOfGenerations { get; set; }
        public int NumberOfAgents { get; set; }
        public int NumberOfSteps { get; set; }

        public GeneticAlgorithm(int numberOfAgents, int numberOfSteps, int numberOfGenerations)
        {
            NumberOfGenerations = numberOfGenerations;
            NumberOfAgents = numberOfAgents;
            NumberOfSteps = numberOfSteps;
            GenerationIdCounter = 1;
            CurrentGeneration = new Generation(0);
        }

        /// <summary>
        /// Initializes the first number of individuals and generates generation 0
        /// </summary>
        /// <param name="numberOfInitialIndividuals">Number of randomly generated individuals in generation 0.</param>
        /// <param name="doc">Revit which is worked in document.</param>
        /// <param name="selfConstruction">Main instance of the programm (Needed to start construction process).</param>
        public abstract void Initialize(int numberOfInitialIndividuals, Document doc, SelfConstruction selfConstruction);
       
        /// <summary>
        /// Selection chooses one of the individuals by Roulette Wheel Selection and puts it straight into the new generation.
        /// </summary>
        /// <returns>A chosen individual, which can be added to the current generation.</returns>
        public abstract Individual Selection(Generation lastGeneration);
        
        /// <summary>
        /// Mutation transforms action by action with a probability of 1/number of Actions.
        /// </summary>
        /// <returns>A chosen and mutated individual, which can be added to the current generation.</returns>
        public abstract Individual Mutate(Generation lastGeneration);
        
        /// <summary>
        /// Crossover chooses two random individuals from the last generation and cuts them into two pieces. 
        /// The pieces get interchanged and put togehter again afterwards.
        /// </summary>
        /// <returns>Two crossovered individuals, which can be added to the current generation.</returns>
        public abstract List<Individual> OnePointCrossover(Generation lastGeneration);
        public abstract double EvaluateFitness(double individualValue, double perfectValue);

        /// <summary>
        /// Individuals with higher fitness get selected wit a large probability 
        /// </summary>
        /// <param name="populationSize"></param>
        /// <param name="totalFitness"></param>
        /// <param name="sortedFitnessTable"></param>
        /// <returns>individuals with highest fitness</returns>
        private int RouletteSelection(int populationSize, double totalFitness, double[] sortedFitnessTable)
        {
            Random random = new Random();
            double randomFitness = random.NextDouble() * totalFitness;
            int idx = -1;
            int mid;
            int first = 0;
            int last = populationSize - 1;
            mid = (last - first) / 2;
            
            //  Binary Search
            while (idx == -1 && first <= last)
            {
                if (randomFitness < sortedFitnessTable[mid])
                {
                    last = mid;
                }
                else if (randomFitness > sortedFitnessTable[mid])
                {
                    first = mid;
                }
                mid = (first + last) / 2;
                if (last - first == 1)
                    idx = last;
            }
            return idx;
        }
    }
}
