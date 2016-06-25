using UnityEngine;
using System.Collections;

public class PlayerState_Idle : BaseState
{
    
    public PlayerState_Idle(StateManager ParentManager)
    {
        Name = "PlayerState_Idle";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void ProcessState()
    {

        bool PressedWalk = Input.GetAxis("Horizontal") != 0;
        bool PressedJump = Input.GetButtonDown("Action");

        if(Input.GetKeyDown("r")){
            MyStateManager.SetNextState("PlayerState_Shmup");
        }

        if(PressedWalk) {
            MyStateManager.SetNextState("PlayerState_Walk");
            return;
        }

        if(Actor.IsGrounded && PressedJump){
            MyStateManager.SetNextState("PlayerState_Jump");
            return;
        }

        if(!Actor.IsGrounded){
            MyStateManager.SetNextState("PlayerState_Fall");
            return;
        }

        if(Actor.FacingRight){
            Actor.Velocity.x -= Actor.GroundDeceleration;

            if (Actor.Velocity.x <= 0) {
                Actor.Velocity.x = 0;
            }
        }

        if(!Actor.FacingRight){
            Actor.Velocity.x += Actor.GroundDeceleration;

            if(Actor.Velocity.x >= 0){
                Actor.Velocity.x = 0;
            }
        }
    }

    public override void ProcessPostFrameState()
    {
        float MaxOffsetDown = Actor.GenerateMaxOffset("DOWN");

        if(MaxOffsetDown <= WorldProperties.GlueHeight){

            Actor.transform.position += new Vector3(0, -MaxOffsetDown, 0);
            MaxOffsetDown = Actor.GenerateMaxOffset("DOWN");
        }
    }

}
