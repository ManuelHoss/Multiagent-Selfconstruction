using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SelfConstruction.AgentCode.Models;
using SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.Abstracts;

namespace SelfConstruction.AgentCode.MovementAlgorithm.ReinforcementLearning.QLearning
{
    public class MovementQLearning : AbstractLearning
    {
        #region Fields
        
        protected internal ConcurrentBag<QValue> QTable;
        protected const double InitQReward = 0.1;

        #endregion

        #region Singleton

        private static MovementQLearning _instance;

        private MovementQLearning()
        {
            // Initialize Q-Table
            QTable = new ConcurrentBag<QValue>();
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
            var qValue = SelectAction(currentState);
            agent.Move(qValue.GetAction());
            UpdateTable(LastQValue, qValue, agent.GetReward());

            LastState = currentState;
            LastQValue = qValue;
        }


        private void UpdateTable(QValue lastQValue, QValue currentQValue, double reward)
        {
            foreach (QValue qValue in QTable)
            {
                if (!qValue.Equals(lastQValue)) continue;
                var newValue = qValue.GetRewardValue() + LearningRate * (reward + Gamma * GetQValueWithBestActionFromQTable(currentQValue.GetState()).GetRewardValue() - qValue.GetRewardValue());
                // Borders for rewardValue 
                // TODO check dimensions
                if (newValue >= -1000000 && newValue <= 1000000)
                {
                    qValue.SetRewardValue(newValue);
                }
            }

        }

        /// <summary>
        /// Decides which MovementAction to choose, based on QTable rewards
        /// </summary>
        /// <param name="state">current state</param>
        /// <param name="qTable">Q Table</param>
        /// <returns>QValue with the best reward for current state</returns>
        public QValue SelectAction(State state)
        {
            //Exploration rate
            var val = new Random().NextDouble();
            
            if (val < .95f)
            {
                // Get the best QValue/MovementAction and return it.
                var bestQValue = GetQValueWithBestActionFromQTable(state);
                return bestQValue;
            }
            else
            {
                var randomAction = SelectRandomAction();
                // Find all QTable for given state.
                var states = QTable.Where(c => c.GetState().Equals(state)).ToArray();
                // Find fitting QValue for randomAction and create a new one if needed.
                return states.FirstOrDefault(s => s.GetAction() == randomAction) ?? CreateNewQValue(state);
            }
        }

        private QValue GetQValueWithBestActionFromQTable(State state)
        {
            QValue[] states = QTable.Where(c => c.GetState().Equals(state)).ToArray();
            if ( states.Length != 0)
            {
                // Find all QValues with matching state
                QValue bestQValue = states[0];
                // Get the QValue/Action with the best rewardValue
                for (int i = states.Length - 1; i >= 0; i--)
                {
                    if (states[i].GetRewardValue() > bestQValue.GetRewardValue())
                    {
                        bestQValue = states[i];
                    }
                }
                return bestQValue;
            }
            // If no QValue is found create a new one
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
