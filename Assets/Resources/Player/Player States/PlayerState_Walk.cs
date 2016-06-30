using UnityEngine;
using System.Collections;

public class PlayerState_Walk : BaseState
{

    bool AutoWalkRight = false;
    bool AutoWalkLeft = false;

    public PlayerState_Walk(StateManager ParentManager)
    {
        Name = "PlayerState_Walk";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void ProcessState()
    {
        bool WalkingRight = Input.GetAxis("Horizontal") > 0f || AutoWalkRight;
        bool WalkingLeft = Input.GetAxis("Horizontal") < 0f || AutoWalkLeft;
        bool PressedJump = Input.GetButtonDown("Action");

        if(Input.GetKeyDown("r")){
            if(Actor.FacingRight){
                AutoWalkRight = !AutoWalkRight;
            } else {
                AutoWalkLeft = !AutoWalkLeft;
            }
        }

        if(!Actor.IsGrounded){
            MyStateManager.SetNextState("PlayerState_Fall");
        }

        if(!WalkingRight && !WalkingLeft){
            MyStateManager.SetNextState("PlayerState_Idle");
        }

        if(PressedJump){
            MyStateManager.SetNextState("PlayerState_Jump");
        }

        if(WalkingRight){

            Actor.Velocity.x += Actor.GroundAcceleration;

            if(Actor.Velocity.x < 0f){
                Actor.Velocity.x += Actor.GroundAcceleration;
            }

            Actor.FacingRight = true;
        }

        if(WalkingLeft){
            Actor.Velocity.x -= Actor.GroundAcceleration;

            if(Actor.Velocity.x > 0f){
                Actor.Velocity.x -= Actor.GroundAcceleration;
            }

            Actor.FacingRight = false;
        }
    }

    public override void ProcessPostFrameState()
    {
        float MaxOffsetDown = Actor.GenerateMaxOffset("DOWN");

        if(MaxOffsetDown <= WorldProperties.GlueHeight){
            Actor.transform.position += new Vector3(0, -MaxOffsetDown, 0);
        }
    }

}
