using UnityEngine;
using System.Collections;

public class PlayerState_TopDownIdle : BaseState
{
    BaseTopViewCharacterController Actor;

    public PlayerState_TopDownIdle(StateManager ParentManager)
    {
        Name = "PlayerState_TopDownIdle";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseTopViewCharacterController>();
    }

    public override void OnEnterState(){

    }

    public override void OnExitState(){

    }

    public override void ProcessState()
    {
        bool PressedUp = Input.GetAxis("Vertical") > 0f;
        bool PressedDown = Input.GetAxis("Vertical") < 0f;
        bool PressedRight = Input.GetAxis("Horizontal") > 0f;
        bool PressedLeft = Input.GetAxis("Horizontal") < 0f;

        if(PressedUp || PressedDown || PressedRight || PressedLeft){
            MyStateManager.SetNextState("PlayerState_TopDownWalk");
        }

        if(Actor.Velocity.x > 0f){
            Actor.Velocity.x -= Actor.Deceleration;

            if(Actor.Velocity.x < 0f){
                Actor.Velocity.x = 0f;
            }
        }

        if(Actor.Velocity.x < 0f){
            Actor.Velocity.x += Actor.Deceleration;

            if(Actor.Velocity.x > 0f){
                Actor.Velocity.x = 0f;
            }
        }

        if(Actor.Velocity.y > 0f){
            Actor.Velocity.y -= Actor.Deceleration;

            if(Actor.Velocity.y < 0f){
                Actor.Velocity.y = 0f;
            }
        }

        if(Actor.Velocity.y < 0f){
            Actor.Velocity.y += Actor.Deceleration;

            if(Actor.Velocity.y > 0f){
                Actor.Velocity.y = 0f;
            }
        }
    }

    public override void ProcessPostFrameState()
    {
    }

}
