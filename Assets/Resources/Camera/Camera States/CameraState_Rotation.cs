using UnityEngine;
using System.Collections;

public class CameraState_Rotation : BaseState
{
    CameraController Actor;
    float RotationSpeed = 1f;
    float MaxRotationSpeed = 50f;
    float Rotation = 0;

    public CameraState_Rotation(StateManager ParentManager)
    {
        Name = "CameraState_Rotation";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<CameraController>();
    }

    public override void OnEnterState(){
        Actor.transform.rotation = Quaternion.Euler(0, 0, 0);
        Rotation = 0;
    }

    public override void OnExitState(){
        Actor.transform.rotation = Quaternion.Euler(0, 0, 0);
        Rotation = 0;
    }

    public override void ProcessState()
    {

        if(Input.GetKeyDown("m")) {
            MyStateManager.SetNextState("CameraState_SnapToPlayer");
        }

        if(Input.GetKeyDown("l")){
            RotationSpeed += 1;
        }

        if(Input.GetKeyDown("k")){
            RotationSpeed -= 1;
        }

        if(RotationSpeed <= 0){
            RotationSpeed = 0;
        }

        if(RotationSpeed >= MaxRotationSpeed){
            RotationSpeed = MaxRotationSpeed;
        }

        Actor.transform.rotation = Quaternion.Euler(0, 0, Rotation);

        Rotation+=RotationSpeed;

        if(Rotation > 360){
            Rotation = 0;
        }

    }

    public override void ProcessPostFrameState()
    {

    }

}
