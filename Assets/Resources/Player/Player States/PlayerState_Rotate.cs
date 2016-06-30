using UnityEngine;
using System.Collections;

public class PlayerState_Rotate : BaseState
{
    float Rotation= 0;
    float RotationSpeed = 1;

    public PlayerState_Rotate(StateManager ParentManager)
    {
        Name = "PlayerState_Rotate";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<BaseSideViewCharacterController>();
    }

    public override void OnEnterState(){
        Rotation = 0;
        RotationSpeed = 1;
    }

    public override void ProcessState()
    {

        Actor.Velocity = new Vector3(0,0,0);
        Actor.Sprite.transform.rotation = Quaternion.Euler(0, 0, Rotation);

        Rotation += RotationSpeed;

        if(Rotation > 360){
            Rotation = 0;
        }

        if(Input.GetKeyDown("o")){
            Actor.Sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            MyStateManager.SetNextState("PlayerState_Idle");
            return;
        }

        if(Input.GetKeyDown("up")){
            RotationSpeed += 1;
        }

        if(Input.GetKeyDown("down")){
            RotationSpeed -= 1;
        }

        if(RotationSpeed <= 0){
            RotationSpeed = 0;
        }

        if(RotationSpeed >= 50){
            RotationSpeed = 50;
        }
    }

}
