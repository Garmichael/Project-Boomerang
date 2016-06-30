using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StateManager {
    public GameObject Actor;
    BaseState CurrentState;
    BaseState NextState;
    string name;

    Dictionary<string, BaseState> AvailableStates = new Dictionary<string, BaseState>();

    public StateManager(string name, GameObject Actor)
    {
        this.name = name;
        this.Actor = Actor;
    }

    public void AddState(BaseState State){
        AvailableStates.Add(State.Name, State);
    }

    public string GetStateName()
    {
        return name;
    }

    public BaseState GetCurrentSate()
    {
        return CurrentState;
    }

    public void UpdateState(){
        if(NextState != null){
            SetState(NextState);
        }
    }

    public void SetNextState(string newStateName){
        NextState = AvailableStates[newStateName];
    }

    private void SetState(BaseState newState)
    {
        BaseState NewState = newState;

        if(CurrentState == NewState)
        {
            return;
        }

        if(CurrentState != null)
        { 
            CurrentState.OnExitState();
        }

        CurrentState = NewState;
        CurrentState.OnEnterState();

        Debug.Log("========== NEW STATE: " + newState.Name + "==========");
    }

    public void ProcessState()
    {
        if (CurrentState != null)
        {
            CurrentState.ProcessState();
        }
    }

    public void ProcessPostFrameState(){
        if (CurrentState != null){
            CurrentState.ProcessPostFrameState();
        }
    }
}
