using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SelfConstruction.AgentCode;

namespace SelfConstruction.GeneticProgrammingCode
{
    public class LogFileWriter
    {
        public void WriteMoveAction(List<Agent> agents)
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

            string fileName = String.Format("{0:s}.creationLog",DateTime.Now).Replace('-','.').Replace(':', '-').Replace('T', ' ');

            File.WriteAllText(Path.Combine(filePath, fileName), stringBuilder.ToString());
        }
    }
}
