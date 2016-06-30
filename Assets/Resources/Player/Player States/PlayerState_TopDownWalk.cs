using UnityEngine;
using System.Collections;

public class PlayerState_TopDownWalk : BaseState
{
    BaseTopViewCharacterController Actor;

    public PlayerState_TopDownWalk(StateManager ParentManager)
    {
        Name = "PlayerState_TopDownWalk";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseTopViewCharacterController>();
    }

    public override void ProcessState()
    {
        bool PressedUp = Input.GetAxis("Vertical") > 0f;
        bool PressedDown = Input.GetAxis("Vertical") < 0f;
        bool PressedRight = Input.GetAxis("Horizontal") > 0f;
        bool PressedLeft = Input.GetAxis("Horizontal") < 0f;

        if(!(PressedUp || PressedDown || PressedRight || PressedLeft)){
            MyStateManager.SetNextState("PlayerState_TopDownIdle");
        }

        if(PressedUp){
            Actor.FacingDirection = "up";
            Actor.Velocity.y += Actor.Acceleration;
            Actor.Velocity.x = 0;
        } else if(PressedRight){
            Actor.FacingDirection = "right";
            Actor.Velocity.x += Actor.Acceleration;
            Actor.Velocity.y = 0;
        } else if(PressedDown){
            Actor.FacingDirection = "down";
            Actor.Velocity.y -= Actor.Acceleration;
            Actor.Velocity.x = 0;
        } else if(PressedLeft){
            Actor.FacingDirection = "left";
            Actor.Velocity.x -= Actor.Acceleration;
            Actor.Velocity.y = 0;
        }

    }

    public override void ProcessPostFrameState()
    {
        float MaxOffsetDown = Actor.GenerateMaxOffset("DOWN");

        if(MaxOffsetDown <= 0f){
            Actor.transform.position += new Vector3(0, -MaxOffsetDown, 0);
        }

        float MaxOffsetUp = Actor.GenerateMaxOffset("UP");

        if(MaxOffsetUp <= 0f){
            Actor.transform.position += new Vector3(0, MaxOffsetUp, 0);
        }

        float MaxOffsetRight= Actor.GenerateMaxOffset("RIGHT");

        if(MaxOffsetRight <= 0f){
            Actor.transform.position += new Vector3(MaxOffsetRight, 0, 0);
        }

        float MaxOffsetLeft = Actor.GenerateMaxOffset("LEFT");

        if(MaxOffsetLeft <= 0f){
            Actor.transform.position += new Vector3(-MaxOffsetLeft, 0, 0);
        }
    }

}
