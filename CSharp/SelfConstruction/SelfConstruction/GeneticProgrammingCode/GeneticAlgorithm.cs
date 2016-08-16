using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Creation;
using SelfConstruction.GeneticProgrammingCode.Logger;
using SelfConstruction.GeneticProgrammingCode.Models;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;
using Document = Autodesk.Revit.DB.Document;

namespace SelfConstruction.GeneticProgrammingCode
{
    public class GeneticAlgorithm
    {
        private Generation _currentGeneration;
        private int _generationIdCounter = 1;
        private int _individualIdCouter = 1;
        private int _numberOfGenerations;
        private int _numberOfAgents;
        private int _numberOfSteps;

        public GeneticAlgorithm(int numberOfAgents, int numberOfSteps, int numberOfGenerations)
        {
            _numberOfGenerations = numberOfGenerations;
            _numberOfAgents = numberOfAgents;
            _numberOfSteps = numberOfSteps;
            _currentGeneration = new Generation(0);
        }

        public void Initialize(int numberOfInitialIndividuals, Document doc, SelfConstruction selfConstruction)
        {
            InstructionUtils instructionUtils = new InstructionUtils();
            for (int i = 0; i < numberOfInitialIndividuals; i++)
            {
                selfConstruction.StartAgentsAndBuildBlocks(doc);

                List<List<Action>> instructionList = instructionUtils.GenerateInstructionsList(selfConstruction.GlobalKnowledge.Agents.ToList());
                // TODO implement fitness function
                double fitnessValue = 0;

                Individual individual = new Individual(_individualIdCouter, _generationIdCounter, instructionList, fitnessValue);
                _currentGeneration.AddIndividual(individual);
                _individualIdCouter++;
            }
            _generationIdCounter++;
        }


    }
}
