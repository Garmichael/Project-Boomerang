using UnityEngine;

public class PlayerState_Jump : BaseState
{

    float TotalJumpDuration = 0;

    public PlayerState_Jump(StateManager ParentManager)
    {
        Name = "PlayerState_Jump";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void OnEnterState()
    {
        TotalJumpDuration = 0;
    }

    public override void ProcessState()
    {
        bool PressingRight = Input.GetAxis("Horizontal") > 0;
        bool PressingLeft = Input.GetAxis("Horizontal") < 0;
        bool StillHoldingJump = Input.GetButton("Action");
        bool ExceededMaximumJumpDuration = TotalJumpDuration >= Actor.MaximumJumpDuration;
        bool HasBonkedHead = Actor.GenerateMaxOffset("UP") <= 0;


        if(Input.GetKeyDown("o")){
            MyStateManager.SetNextState("PlayerState_Rotate");
            return;
        }

        if(!PressingLeft && !PressingRight){
            if(Actor.FacingRight){
                if(Actor.Velocity.x > 0f){
                    Actor.Velocity.x -= Actor.AirDecelerationForward;
                    if (Actor.Velocity.x <= 0f) {
                        Actor.Velocity.x = 0f;
                    }
                } else {
                    Actor.Velocity.x += Actor.AirDecelerationBackward;
                    if (Actor.Velocity.x >= 0f) {
                        Actor.Velocity.x = 0f;
                    }
                }
            }

            if(!Actor.FacingRight){
                if(Actor.Velocity.x < 0f){
                    Actor.Velocity.x += Actor.AirDecelerationForward;
                    if (Actor.Velocity.x >= 0f) {
                        Actor.Velocity.x = 0f;
                    }
                } else {
                    Actor.Velocity.x -= Actor.AirDecelerationBackward;
                    if (Actor.Velocity.x <= 0f) {
                        Actor.Velocity.x = 0f;
                    }
                }
            }
        }


        if(PressingRight){
            if(Actor.FacingRight){
                Actor.Velocity.x += Actor.AirAccelerationForward;
            } else {
                Actor.Velocity.x += Actor.AirAccelerationBackward;
            }

            if(Actor.CanChangeDirectionInMidAir){
                Actor.FacingRight = true;
            }
        }

        if(PressingLeft){
            if(!Actor.FacingRight){
                Actor.Velocity.x -= Actor.AirAccelerationForward;
            } else {
                Actor.Velocity.x -= Actor.AirAccelerationBackward;
            }

            if(Actor.CanChangeDirectionInMidAir){
                Actor.FacingRight = false;
            }
        }

        if(StillHoldingJump && !ExceededMaximumJumpDuration && !HasBonkedHead){
            Actor.Velocity.y = Actor.JumpStrength;
            TotalJumpDuration += WorldProperties.GetDeltaTime();
        } else {
            if(HasBonkedHead){
                Actor.Velocity.y = 0;
            }

            MyStateManager.SetNextState("PlayerState_Fall");
        }
    }
}
