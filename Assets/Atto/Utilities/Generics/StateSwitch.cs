using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atto;

public enum SwitchStates { Undefined, Off, On }

public class StateSwitch : StateMachine<SwitchStates>
{
    bool locked = false;

    public StateSwitch(System.Action OnTurnOn, System.Action OnTurnOff, SwitchStates initialState)
    {
        AddState(SwitchStates.Undefined, () => { });
        AddState(SwitchStates.On, OnTurnOn);
        AddState(SwitchStates.Off, OnTurnOff);
        CurrentState = initialState;
    }

    public void TurnOn()
    {
        if (CurrentState == SwitchStates.Off)
        {
            if (!locked)
            {
                CurrentState = SwitchStates.On;
            }
            else
            {
                Debug.Log("Can't turn switch ON because it's LOCKED");
            }
        }
        else
        {
            Debug.Log("Can't turn switch on because it's ALREADY ON");
        }
    }

    public void TurnOff()
    {
        if (CurrentState == SwitchStates.On)
        {
            if (!locked)
            {
                CurrentState = SwitchStates.Off;
            }
            else
            {
                Debug.Log("Can't turn switch ON because it's LOCKED");
            }
        }
        else
        {
            Debug.Log("Can't turn switch off because it's ALREADY OFF");
        }
    }

    /// <summary>
    /// Toggle the switch to the opposite value of the current one
    /// </summary>
    public void Toggle()
    {
        if (!locked)
        {
            if (CurrentState == SwitchStates.On)
            {
                CurrentState = SwitchStates.Off;
            }
            else
            {
                CurrentState = SwitchStates.On;
            }
        }
        else
        {
            Debug.Log("Can't toggle switch because it's LOCKED");
        }
    }

}
