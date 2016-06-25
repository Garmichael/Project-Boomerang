using UnityEngine;
using System.Collections;

public class PlayerState_Fall : BaseState
{

    public PlayerState_Fall(StateManager ParentManager)
    {
        Name = "PlayerState_Fall";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void ProcessState()
    {
        bool PressingRight = Input.GetAxis("Horizontal") > 0f;
        bool PressingLeft = Input.GetAxis("Horizontal") < 0f;

        if(Input.GetKeyDown("o")){
            MyStateManager.SetNextState("PlayerState_Rotate");
            return;
        }

        if(!Actor.IsGrounded){
            if(Actor.Velocity.y > -WorldProperties.TerminalVelocity){
                Actor.Velocity.y += -WorldProperties.Gravity;
            }
        } else {
            MyStateManager.SetNextState("PlayerState_Idle");
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

    }

    public override void ProcessPostFrameState(){
        float MaxOffsetDown = Actor.GenerateMaxOffset("DOWN");
        float MaxOffsetUp = Actor.GenerateMaxOffset("UP");
        float MaxOffsetRight = Actor.GenerateMaxOffset("RIGHT");
        float MaxOffsetLeft = Actor.GenerateMaxOffset("LEFT");

        if(MaxOffsetDown <= WorldProperties.JumpingGlueHeight){
            Actor.transform.position += new Vector3(0, -MaxOffsetDown, 0);
        }

        if(MaxOffsetUp <= 0 && Actor.Velocity.y > 0){
            Actor.transform.position += new Vector3(0, MaxOffsetUp, 0);
            Actor.Velocity.y = 0;
        }
    }

}
