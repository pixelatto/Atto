using System;
using System.Collections.Generic;

public class StateMachine<TLabel>
{
	#region  Types

	private class State
	{
		#region Public Fields

		public readonly TLabel label;
		public readonly Action onStart;
		public readonly Action onStop;
		public readonly Action onUpdate;

		#endregion

		#region Constructors

		public State(TLabel label, Action onStart, Action onUpdate, Action onStop)
		{
			this.onStart = onStart;
			this.onUpdate = onUpdate;
			this.onStop = onStop;
			this.label = label;
		}

		#endregion
	}

	#endregion

	#region Private Fields

	private readonly Dictionary<TLabel, State> stateDictionary;
	private State currentState;

	#endregion

	#region  Properties

	/// <summary>
	/// Returns the label of the current state.
	/// </summary>
	public TLabel CurrentState
	{
		get { return currentState.label; }
		
		set { ChangeState(value); }
	}

	#endregion

	#region Constructors

	/// <summary>
	/// Constructs a new StateMachine.
	/// </summary>
	public StateMachine()
	{
		stateDictionary = new Dictionary<TLabel, State>();
	}

	#endregion

	#region Unity Callbacks

	/// <summary>
	/// This method should be called every frame. 
	/// </summary>
	public void Update()
	{
		if (currentState != null && currentState.onUpdate != null)
		{
			currentState.onUpdate();
		}
	}

	#endregion

	#region Public Methods

	/// <summary>
	/// Adds a state, and the delegates that should run 
	/// when the state starts, stops, 
	/// and when the state machine is updated.
	/// 
	/// Any delegate can be null, and wont be executed.
	/// </summary>
	/// <param name="label">The name of the state to add.</param>
	/// <param name="onStart">The action performed when the state is entered.</param>
	/// <param name="onUpdate">The action performed when the state machine is updated in the given state.</param>
	/// <param name="onStop">The action performed when the state machine is left.</param>
	public void AddState(TLabel label, Action onStart, Action onUpdate, Action onStop)
	{
		stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
	}

	public void AddState(TLabel label, Action onStart, Action onUpdate)
	{
		AddState(label, onStart, onUpdate, null);
	}

	public void AddState(TLabel label, Action onStart)
	{
		AddState(label, onStart, null);
	}

	public void AddState(TLabel label)
	{
		AddState(label, null);
	}

	public void AddState<TSubstateLabel>(TLabel label, StateMachine<TSubstateLabel> subMachine,
		TSubstateLabel subMachineStartState)
	{
		AddState(
			label,
			() => subMachine.ChangeState(subMachineStartState),
			subMachine.Update);
	}
	
	public override string ToString()
	{
		return CurrentState.ToString();
	}

	#endregion

	#region Private Methods

	/// <summary>
	/// Changes the state from the existing one to the state with the given label.
	/// 
	/// It is legal (and useful) to transition to the same state, in which case the 
	/// current state's onStop action is called, the onstart ation is called, and the
	/// state keeps on updating as before. The behviour is exactly the same as switching to
	/// a new state.
	/// </summary>
	/// <param name="newState"></param>
	private void ChangeState(TLabel newState)
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

	#endregion
}
