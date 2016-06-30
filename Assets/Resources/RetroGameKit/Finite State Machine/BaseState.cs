using System.Collections;
using UnityEngine;

public class BaseState
{

    protected StateManager MyStateManager;
    public string Name = "Unnamed_State";
    protected BaseSideViewCharacterController Actor;

    public BaseState(){}
    public virtual void OnEnterState(){}
    public virtual void OnExitState(){}
    public virtual void ProcessState(){}
    public virtual void ProcessPostFrameState(){}
}
