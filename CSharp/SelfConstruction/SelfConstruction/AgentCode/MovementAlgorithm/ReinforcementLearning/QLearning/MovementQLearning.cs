using System;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning
{
    public class MovementQLearning : AbstractLearning
    {
        #region Fields
        
        protected internal List<QValue> QTable;
        protected const double InitQReward = 0.1;

        #endregion

        #region Singleton

        private static MovementQLearning _instance;

        private MovementQLearning()
        {
            // Initialize Q-Table
            QTable = new List<QValue>();
            State initialState = new State(new Position(0, 0, 0));
            QTable.Add(new QValue(initialState, MovementAction.MoveBackward, InitQReward));
            QTable.Add(new QValue(initialState, MovementAction.MoveForward, InitQReward));
            QTable.Add(new QValue(initialState, MovementAction.MoveDown, InitQReward));
            QTable.Add(new QValue(initialState, MovementAction.MoveUp, InitQReward));
            QTable.Add(new QValue(initialState, MovementAction.MoveLeft, InitQReward));
            QTable.Add(new QValue(initialState, MovementAction.MoveRight, InitQReward));
        }

        public static MovementQLearning Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MovementQLearning();
                }
                return _instance;
            }
        }

        #endregion
        
        public override void ExecuteMovement(Agent agent, State currentState)
        {
            var qValue = SelectAction(currentState, QTable);
            agent.Move(qValue.GetAction());
            QTable = UpdateTable(LastQValue, qValue, QTable, agent.GetReward());

            LastState = currentState;
            LastQValue = qValue;
        }

        private List<QValue> UpdateTable(QValue lastQValue, QValue currentQValue, List<QValue> qTable, double reward)
        {
            for (int i = qTable.Count - 1; i >= 0; i--)
            {
                var qValueToBeUpdated = qTable.ElementAt(i);

                if (!qValueToBeUpdated.Equals(lastQValue)) continue;
                var newValue = qValueToBeUpdated.GetRewardValue() + LearningRate * (reward + Gamma * GetQValueWithBestActionFromQTable(currentQValue.GetState(), qTable).GetRewardValue() - qValueToBeUpdated.GetRewardValue());
                if (newValue >= -1000000 && newValue <= 1000000)
                {
                    qValueToBeUpdated.SetRewardValue(newValue);
                }
            }
            return qTable;
        }

        /// <summary>
        /// Decides which MovementAction to choose, based on QTable rewards
        /// </summary>
        /// <param name="state">current state</param>
        /// <param name="qTable">Q Table</param>
        /// <returns>QValue with the best reward for current state</returns>
        public QValue SelectAction(State state, List<QValue> qTable)
        {
            //Exploration rate
            var val = new Random().NextDouble();
            
            if (val < .95f)
            {
                // Get the best QValue/MovementAction and return it.
                var bestQValue = GetQValueWithBestActionFromQTable(state, qTable);
                return bestQValue;
            }
            else
            {
                var randomAction = SelectRandomAction();
                // Find all QTable for given state.
                var states = qTable.FindAll(c => c.GetState().Equals(state)).ToArray();
                // Find fitting QValue for randomAction and create a new one if needed.
                var bestQValue = states.FirstOrDefault(s => s.GetAction() == randomAction);
                if (bestQValue == null)
                {
                    bestQValue = CreateNewQValue(state);
                }
                return bestQValue;
            }
        }

        private QValue GetQValueWithBestActionFromQTable(State state, List<QValue> qTable)
        {
            var states = qTable.FindAll(c => c.GetState().Equals(state)).ToArray();
            if (states.Length > 0)
            {
                QValue bestQValue = states[0];
                foreach (var qValue in states)
                {
                    if (!bestQValue.Equals(qValue) && qValue.GetRewardValue() > bestQValue.GetRewardValue())
                    {
                        bestQValue = qValue;
                    }
                }
                return bestQValue;
            }
            QValue newQValue = CreateNewQValue(state);
            return newQValue;
        }

        public QValue CreateNewQValue(State state)
        {
            QValue newQValue = new QValue(state, SelectRandomAction(), InitQReward);
            QTable.Add(newQValue);
            return newQValue;
        }
    }
}
