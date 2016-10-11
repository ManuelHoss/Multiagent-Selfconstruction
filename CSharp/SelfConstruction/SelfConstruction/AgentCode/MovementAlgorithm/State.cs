using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.MovementAlgorithm
{
    /// <summary>
    /// Class that represents the current situation of an Agent.
    /// </summary>
    public class State
    {
        public readonly Position PheromoneIntensity;
        
        public State(Position pheromoneIntensity)
        {
            PheromoneIntensity = pheromoneIntensity;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is State)) { return false; }

            return ((State)obj).PheromoneIntensity.Equals(PheromoneIntensity);
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
