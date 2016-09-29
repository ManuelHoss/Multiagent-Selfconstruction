using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Autodesk.Revit.DB;
using SelfConstruction.AgentCode;
using SelfConstruction.GeneticProgrammingCode.Models;
using Action = SelfConstruction.GeneticProgrammingCode.Models.Action;

namespace SelfConstruction.GeneticProgrammingCode.Logger
{
    public class InstructionUtils
    {
        

        public List<List<Action>> GenerateInstructionsList(List<Agent> agents)
        {
            List<List<Action>> instructionsList = new List<List<Action>>();
            // Iterate agents and string --> Convert strings into Actions
            for(int i = 0; i < agents.Count; i++)
            {
                var actionStrings = agents[i].logString.Split('|');
                for (int j = 0; j < actionStrings.Length; j++)
                {
                    instructionsList.Add(ConvertActionSequenceStringArrayToActionQueue(actionStrings));
                }
            }
            return instructionsList;
        }

        private List<Action> ConvertActionSequenceStringArrayToActionQueue(string[] actionSequenceStringArray)
        {
            List<Action> actionQueue = new List<Action>();

            foreach (string actionString in actionSequenceStringArray)
            {
                if (actionString.Contains("BUILD"))
                {
                    actionQueue.Add(new Action(ActionType.Build));
                }
                else if (actionString.Contains("SPACE"))
                {
                    actionQueue.Add(new Action(ActionType.PlaceSpacePheromone));
                }
                else if(actionString.Contains("MOVE"))
                {
                    // Convert MOVE(X,Y,Z) to XYZ object
                    char[] actionStringCharArray = actionString.ToCharArray();
                    double deltaX = actionStringCharArray[6];
                    double deltaY = actionStringCharArray[8];
                    double deltaZ = actionStringCharArray[10];
                    XYZ deltaPosition = new XYZ(deltaX, deltaY, deltaZ);

                    actionQueue.Add(new Action(ActionType.Move, deltaPosition));
                }
                else { }
            }
            return actionQueue;
        }

        // WRITE TO FILE
        public void WriteActionSequenceToFile(List<Agent> agents)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CreationLogs\";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            var stringBuilder = new StringBuilder();

            foreach (Agent agent in agents)
            {
                string newLine = agent.logString;
                stringBuilder.AppendLine(newLine);
            }

            string fileName = String.Format("{0:s}.creationLog", DateTime.Now).Replace('-', '.').Replace(':', '-').Replace('T', ' ');

            File.WriteAllText(Path.Combine(filePath, fileName), stringBuilder.ToString());
        }

        // READ FROM FILE
        public List<string[]> ReadActionSequenceFromFile()
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\CreationLogs\";
            var reader = new StreamReader(File.OpenRead(@filePath + "2016.08.13 14-30-34.creationLog"));
            // Each string array represents the sequence one agent has to perform
            List<string[]> actionStringsList = new List<string[]>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split('|');
                actionStringsList.Add(values);
            }
            return actionStringsList;
        }
    }
}
