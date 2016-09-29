namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts
{
    public abstract class AbstractValue
    {
        //_state the ki is in
        private readonly State _state;

        // _movementAction to choose
        private readonly MovementAction _movementAction;

        // rewardValue the ki gets when choosing the _movementAction
        private double _rewardValue;

        protected AbstractValue(State state, MovementAction movementAction, double rewardValue)
        {
            _state = state;
            _movementAction = movementAction;
            _rewardValue = rewardValue;
        }

        public void SetRewardValue(double reward)
        {
            _rewardValue = reward;
        }

        public double GetRewardValue()
        {
            return _rewardValue;
        }

        public MovementAction GetAction()
        {
            return _movementAction;
        }

        public State GetState()
        {
            return _state;
        }
    }
}
