using UnityEngine;
using System.Collections;

public class PlayerState_Shmup : BaseState
{

    public PlayerState_Shmup(StateManager ParentManager)
    {
        Name = "PlayerState_Shmup";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void OnEnterState(){
        Actor.CollidesWithGeometry = false;
    }

    public override void OnExitState(){
        Actor.CollidesWithGeometry = true;
    }

    public override void ProcessState()
    {
        bool PressedUp = Input.GetAxis("Vertical") > 0f;
        bool PressedDown = Input.GetAxis("Vertical") < 0f;
        bool PressedJump = Input.GetButtonDown("Action");

        Actor.Velocity.x += Actor.AirAccelerationForward;

        if(PressedUp){
            Actor.Velocity.y += Actor.AirAccelerationForward;
        } else if (PressedDown){
            Actor.Velocity.y -= Actor.AirAccelerationForward;
        } else {
            Actor.Velocity.y = 0;
        }

        if(PressedJump){
            MyStateManager.SetNextState("PlayerState_Idle");
        }

    }

    public override void ProcessPostFrameState()
    {
    }

}
