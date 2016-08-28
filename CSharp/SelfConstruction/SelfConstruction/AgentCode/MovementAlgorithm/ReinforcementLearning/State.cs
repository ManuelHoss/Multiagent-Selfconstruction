using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning
{
    /// <summary>
    /// Class that represents the current situation of an Agent.
    /// </summary>
    public class State
    {
        public readonly Position Position;
        
        public State(Position position)
        {
            Position = position;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State)) { return false; }

            return ((State)obj).Position.Equals(Position);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}", base.ToString());
        }
    }
}
