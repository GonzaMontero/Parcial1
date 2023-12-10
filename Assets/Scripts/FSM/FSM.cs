using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class FSMParameters
    {
        //Assistant class that lets us pass parameters easily (and store them) when entering a state (instead of sending 500 refs)
        public object[] Parameters { get; set; }
    }

    public class FSM
    {
        private int currentState;

        private Dictionary<int, FSMAction> states;
        private Dictionary<int, FSMParameters> onEnterParameters; //Stores all parameters needed when entering a new state
        private Dictionary<int, FSMParameters> onExecuteParameters; //Stores all parameters needed when executing a state
        private Dictionary<int, FSMParameters> onExitParameters; //Stores all parameters needed when exiting a state

        private int[,] relations;

        public FSM(int states, int flags)
        {
            currentState = -1;

            relations = new int[states, flags];
            for (int i = 0; i < states; i++)
            {
                for (int j = 0; j < flags; j++)
                {
                    relations[i, j] = -1;
                }
            }

            this.states = new Dictionary<int, FSMAction>();
            onEnterParameters = new Dictionary<int, FSMParameters>();
            onExecuteParameters = new Dictionary<int, FSMParameters>();
            onExitParameters = new Dictionary<int, FSMParameters>();
        }

        public FSM()
        {
        }

        public void ForceCurrentState(int state)
        {
            currentState = state;

            foreach (Action OnEnterAction in states[currentState].OnEnterBehaviours(onEnterParameters[currentState]))
                OnEnterAction?.Invoke();
        }

        public void SetRelation(int sourceState, int flag, int destinationState)
        {
            relations[sourceState, flag] = destinationState;
        }

        public void SetFlag(int flag)
        {
            if (relations[currentState, flag] != -1)
            {
                foreach (Action OnExitAction in states[currentState].OnExitBehaviours(onEnterParameters[currentState]))
                    OnExitAction?.Invoke();

                currentState = relations[currentState, flag];

                foreach (Action OnEnterAction in states[currentState].OnEnterBehaviours(onEnterParameters[currentState]))
                    OnEnterAction?.Invoke();
            }
        }

        public void SetAction<T>(int stateIndex, FSMParameters onExecuteParams = null, FSMParameters onEnterParams = null,
            FSMParameters onExitParams = null) where T : FSMAction, new()
        {
            if (states.ContainsKey(stateIndex))
            {
                FSMAction newAction = new T();
                newAction.OnSetFlag += SetFlag;
                states.Add(stateIndex, newAction);
                onEnterParameters.Add(stateIndex, onEnterParams);
                onExecuteParameters.Add(stateIndex, onExecuteParams);
                onExitParameters.Add(stateIndex, onExitParams);
            }
        }

        public void Update()
        {
            if (states.ContainsKey(currentState))
            {
                foreach(Action OnExecute in states[currentState].OnExecuteBehaviours(onExecuteParameters[currentState]))
                {
                    OnExecute?.Invoke();
                }
            }
        }

        public int GetCurrentState()
        {
            return currentState;
        }
    }
}