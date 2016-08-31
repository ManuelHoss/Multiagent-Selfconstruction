using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning
{
    public class Movement
    {
        #region Fields


        List<Position> lastActions = new List<Position>();
        private bool _decrement = false;
   

        #endregion

        public object ChooseMovementAction(Position currentPosition)
        {
            object movement = GetRandomMovementAction();
            if (!_decrement)
            {
                movement = GetRandomMovementAction();
                lastActions.Add(currentPosition);
                _decrement = lastActions.Count == 20;
            }
            else if (_decrement && lastActions.Any())
            {
                movement = lastActions.Last();
                lastActions.Remove(lastActions.Last());
            }
            else 
            {
                _decrement = false;
                lastActions = new List<Position>();
                movement = GetRandomMovementAction();
                lastActions.Add(currentPosition);

            }
            Console.WriteLine(movement.ToString());
            return movement;
        }

        public void SetBuild()
        {
            _decrement = false;
            lastActions = new List<Position>();
        }

        private MovementAction GetRandomMovementAction()
        {
            Array values = Enum.GetValues(typeof(MovementAction));
            Random random = new Random();
            return (MovementAction) values.GetValue(random.Next(values.Length));
        }
        

        private MovementAction getOppositeMovementAction(MovementAction move)
        {
            MovementAction movementAction;
            switch (move)
            {
                case MovementAction.MoveBackward:
                    movementAction = MovementAction.MoveForward;
                    break;
                case MovementAction.MoveForward:
                    movementAction = MovementAction.MoveBackward;
                    break;
                case MovementAction.MoveUp:
                    movementAction = MovementAction.MoveDown;
                    break;
                case MovementAction.MoveDown:
                    movementAction = MovementAction.MoveUp;
                    break;
                case MovementAction.MoveLeft:
                    movementAction = MovementAction.MoveRight;
                    break;
                case MovementAction.MoveRight:
                    movementAction = MovementAction.MoveLeft;
                    break;
                default:
                    movementAction = MovementAction.MoveUp;
                    break;
            }

            return movementAction;
        }
    }
}
