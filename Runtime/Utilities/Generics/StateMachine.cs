using System;
using System.Collections.Generic;

public class StateMachine<T>
{
	private class State
	{
		public readonly T label;
		public readonly Action onStart;
		public readonly Action onStop;
		public readonly Action onUpdate;

		public State(T label, Action onStart, Action onUpdate, Action onStop)
		{
			this.onStart = onStart;
			this.onUpdate = onUpdate;
			this.onStop = onStop;
			this.label = label;
		}
	}

	private readonly Dictionary<T, State> stateDictionary;
	private State currentState;

	public T CurrentState
	{
		get { return currentState.label; }
		set { ChangeState(value); }
	}

	public StateMachine()
	{
		stateDictionary = new Dictionary<T, State>();
	}

	public void Update()
	{
		if (currentState != null && currentState.onUpdate != null)
		{
			currentState.onUpdate();
		}
	}

	public void AddState(T label, Action onStart, Action onUpdate, Action onStop)
	{
		stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
	}

	public void AddState(T label, Action onStart, Action onUpdate)
	{
		AddState(label, onStart, onUpdate, null);
	}

	public void AddState(T label, Action onStart)
	{
		AddState(label, onStart, null);
	}

	public void AddState(T label)
	{
		AddState(label, null);
	}

	public void AddState<TSubstateLabel>(T label, StateMachine<TSubstateLabel> subMachine, TSubstateLabel subMachineStartState)
	{
		AddState(label, () => subMachine.ChangeState(subMachineStartState), subMachine.Update);
	}

	public override string ToString()
	{
		return CurrentState.ToString();
	}

	private void ChangeState(T newState)
	{
		if (currentState != null && currentState.onStop != null)
		{
			currentState.onStop();
		}

		currentState = stateDictionary[newState];

		if (currentState.onStart != null)
		{
			currentState.onStart();
		}
	}
}
