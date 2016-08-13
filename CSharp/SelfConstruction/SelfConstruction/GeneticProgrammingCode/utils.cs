using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SelfConstruction.GeneticProgrammingCode.Models;

namespace SelfConstruction.GeneticProgrammingCode
{
    public class Utils
    {
        private Queue<Action> convertActionSequenceStringArrayToActionQueue(string[] actionSequenceStringArray)
        {
            Queue<Action> actionQueue = new Queue<Action>();

            foreach (string actionString in actionSequenceStringArray)
            {
                if (actionString.Contains("BUILD"))
                {
                    actionQueue.Enqueue(new Action(ActionType.Build));
                }
                else if (actionString.Contains("MOVE"))
                {
                    // Convert MOVE(X,Y,Z) to XYZ object
                    char[] actionStringCharArray = actionString.ToCharArray();
                    double deltaX = actionStringCharArray[5];
                    double deltaY = actionStringCharArray[7];
                    double deltaZ = actionStringCharArray[9];
                    XYZ deltaPosition = new XYZ(deltaX, deltaY, deltaZ);

                    actionQueue.Enqueue(new Action(ActionType.Move, deltaPosition));
                }
            }
            return actionQueue;
        }
    }
}
