using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SelfConstruction.GeneticProgrammingCode.Logger;
using SelfConstruction.GeneticProgrammingCode.Models;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;

namespace SelfConstruction.GeneticProgrammingCode.GeneticAlgorithm
{
    public class GeneticAlgorithmVolumeOptimization : GeneticAlgorithm
    {
        public double PerfectVolume { get; }

        public GeneticAlgorithmVolumeOptimization(int numberOfAgents, int numberOfSteps, int numberOfGenerations, double perfectVolume) : base(numberOfAgents, numberOfSteps, numberOfGenerations)
        {
            PerfectVolume = perfectVolume;
        }

        public override void Initialize(int numberOfInitialIndividuals, Document doc, SelfConstruction selfConstruction)
        {
            InstructionUtils instructionUtils = new InstructionUtils();
            for (int i = 0; i < numberOfInitialIndividuals; i++)
            {
                // Execute build process
                selfConstruction.StartAgentsAndBuildBlocks(doc);
                //TODO get volume
                double individualVolume = 0;

                // Parse instruction string into a list of actions for every agent
                List<List<Action>> instructionList = instructionUtils.GenerateInstructionsList(selfConstruction.GlobalKnowledge.Agents.ToList());

                // Evaluate fitness value
                double fitnessValue = EvaluateFitness(individualVolume, PerfectVolume);

                // Create individual and add it to the current generation
                IndividualVolumeOptimization individual = new IndividualVolumeOptimization(this.IndividualIdCouter, GenerationIdCounter, instructionList, fitnessValue, individualVolume);
                this.CurrentGeneration.AddIndividual(individual);
                IndividualIdCouter++;
            }
            GenerationIdCounter++;
        }

        public override Individual Selection()
        {
            throw new NotImplementedException();
        }

        public override Individual Mutate()
        {
            throw new NotImplementedException();
        }

        public override List<Individual> OnePointCrossover()
        {
            throw new NotImplementedException();
        }

        public override double EvaluateFitness(double individualValue, double perfectValue)
        {
            double fitnessValue = 0;

            // FitnessValue is the absolute error to the perfect value
            fitnessValue = Math.Abs(perfectValue - individualValue);

            return fitnessValue;
        }
    }
}
