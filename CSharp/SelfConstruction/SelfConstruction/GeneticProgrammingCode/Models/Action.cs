using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using SelfConstruction.AgentCode;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.GeneticProgrammingCode.Models;

namespace SelfConstruction.GeneticProgrammingCode
{
    public class Action
    {
        public ActionType ActionType;
        public XYZ DeltaPosition;

        public Action(ActionType actionType = ActionType.Build)
        {
            this.ActionType = actionType;
        }

        public Action(ActionType actionType = ActionType.Move, XYZ deltaPosition = null)
        {
            this.ActionType = actionType;
            DeltaPosition = deltaPosition;
        }

    }
}
