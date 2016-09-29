using Autodesk.Revit.DB;

namespace SelfConstruction.GeneticProgrammingCode.Models
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
