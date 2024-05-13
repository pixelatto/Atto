using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateMachine<StateLabel>
{
    public StateLabel currentStateLabel;
    Dictionary<StateLabel, State> states = new Dictionary<StateLabel, State>();

    public float timeInCurrentState => Time.time - currentStateEnterTime;
    float currentStateEnterTime = 0;

    public void ChangeState(StateLabel newStateLabel)
    {
        if (!Equals(newStateLabel, currentStateLabel))
        {
            states[currentStateLabel].OnStateExit.InvokeIfNotNull();
            currentStateLabel = newStateLabel;
            states[currentStateLabel].OnStateEnter.InvokeIfNotNull();
            currentStateEnterTime = Time.time;
        }
    }

    public void Update()
    {
        if (states.ContainsKey(currentStateLabel))
        {
            states[currentStateLabel].OnStateUpdate.InvokeIfNotNull();
        }
        else
        {
            Debug.LogWarning("Invalid state " + currentStateLabel);
        }
    }

    public void AddState(StateLabel newState, System.Action OnUpdate)
    {
        states.Add(newState, new State() { OnStateUpdate = OnUpdate });
    }

    public void AddState(StateLabel newState, System.Action OnUpdate, System.Action OnEnter)
    {
        states.Add(newState, new State() { OnStateUpdate = OnUpdate, OnStateEnter = OnEnter });
    }

    public void AddState(StateLabel newState, System.Action OnUpdate, System.Action OnEnter, System.Action OnExit)
    {
        states.Add(newState, new State() { OnStateUpdate = OnUpdate, OnStateEnter = OnEnter, OnStateExit = OnExit });
    }
}
