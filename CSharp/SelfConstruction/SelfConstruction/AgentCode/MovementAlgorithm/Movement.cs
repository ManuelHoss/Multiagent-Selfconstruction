using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Models;

namespace SelfConstruction.AgentCode.MovementAlgorithm
{
   /// <summary>
   /// Class Movement.
   /// </summary>
   public class Movement
   {
      #region Fields

      List<Position> _lastActions = new List<Position>();
      private bool _decrement = false;
      private int _maxStepsWithoutBuild = new Random().Next(15, 25);

      #endregion

      /// <summary>
      /// Chooses the movement action depending on last steps
      /// if there where many steps without a build, the agent should walk backwards
      /// </summary>
      /// <param name="currentPosition">The current position.</param>
      /// <returns></returns>
      public object ChooseMovementAction(Position currentPosition)
      {
         object movement = GetRandomMovementAction();
         if (!_decrement)
         {
            movement = GetRandomMovementAction();
            _lastActions.Add(currentPosition);
            _decrement = _lastActions.Count == _maxStepsWithoutBuild;
         }
         else if (_decrement && _lastActions.Any())
         {
            movement = _lastActions.Last();
            _lastActions.Remove(_lastActions.Last());
         }
         else
         {
            _decrement = false;
            _lastActions = new List<Position>();
            movement = GetRandomMovementAction();
            _lastActions.Add(currentPosition);
         }
         Console.WriteLine(movement.ToString());
         return movement;
      }


      /// <summary>
      /// Resets the stepcounter because of a buildaction
      /// </summary>
      public void ResetCauseOfBuild()
      {
         _decrement = false;
         _lastActions = new List<Position>();
         _maxStepsWithoutBuild = new Random().Next(5, 40);
      }

      /// <summary>
      /// Gets the random movement action.
      /// </summary>
      /// <returns></returns>
      private MovementAction GetRandomMovementAction()
      {
         Array values = Enum.GetValues(typeof(MovementAction));
         Random random = new Random();
         return (MovementAction) values.GetValue(random.Next(values.Length));
      }
   }
}