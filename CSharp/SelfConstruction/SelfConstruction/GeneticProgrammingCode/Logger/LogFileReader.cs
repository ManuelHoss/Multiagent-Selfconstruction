using System;
using System.Collections.Generic;
using System.IO;

namespace SelfConstruction.GeneticProgrammingCode.Logger
{
    public class LogFileReader
    {
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
