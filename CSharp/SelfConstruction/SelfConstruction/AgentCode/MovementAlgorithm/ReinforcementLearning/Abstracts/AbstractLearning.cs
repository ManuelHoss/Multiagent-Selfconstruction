using System;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts
{
    public abstract class AbstractLearning
    {
        public const double Gamma = 0.9;
        public const double LearningRate = 0.2;
        public const double Lambda = 0.9;

        protected State LastState;
        protected QValue LastQValue;

        public abstract void ExecuteMovement(Agent agent, State currentState);

        /// <summary>
        /// Selects and returns a randomly choosen MovementAction.
        /// </summary>
        /// <returns>Hit or Stand</returns>
        protected static MovementAction SelectRandomAction()
        {
            int random = new Random().Next(6);
            MovementAction movementAction;

            switch (random)
            {
                case 0:
                    movementAction = MovementAction.MoveBackward;
                    break;
                case 1:
                    movementAction = MovementAction.MoveForward;
                    break;
                case 2:
                    movementAction = MovementAction.MoveDown;
                    break;
                case 3:
                    movementAction = MovementAction.MoveUp;
                    break;
                case 4:
                    movementAction = MovementAction.MoveLeft;
                    break;
                case 5:
                    movementAction = MovementAction.MoveRight;
                    break;
                default:
                    movementAction = MovementAction.MoveForward;
                    break;
            }

            return movementAction;
        }
    }
}
