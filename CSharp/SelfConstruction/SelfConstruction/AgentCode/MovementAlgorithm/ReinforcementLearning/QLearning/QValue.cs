using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning
{
    public class QValue : AbstractValue
    {
        public QValue(State state, MovementAction movementAction, double reward) : base(state, movementAction, reward)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(QValue)) return false;

            return GetState().Equals(((QValue)obj).GetState()) && GetAction() == ((QValue)obj).GetAction();
        }
    }
}
