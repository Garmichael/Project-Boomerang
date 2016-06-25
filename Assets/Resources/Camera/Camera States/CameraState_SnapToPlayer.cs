using UnityEngine;
using System.Collections;

public class CameraState_SnapToPlayer : BaseState
{
    CameraController Actor;

    public CameraState_SnapToPlayer(StateManager ParentManager)
    {
        Name = "CameraState_SnapToPlayer";
        MyStateManager = ParentManager;
        Actor = MyStateManager.Actor.GetComponent<CameraController>();
    }

    public override void ProcessState()
    {

        float verticalExtent = Actor.Camera.orthographicSize;
        float horizontalExtent = verticalExtent * Screen.width / Screen.height;

        if(Input.GetKeyDown("m")){
            MyStateManager.SetNextState("CameraState_Rotation");
        }

        if(Actor.CurrentView == null){
            return;
        }

        Actor.DestinationPosition = Actor.Player.transform.position + new Vector3(0, 2, -25);

        if(Actor.DestinationPosition.x - horizontalExtent < Actor.CurrentView.LeftExtent){
            Actor.DestinationPosition.x = Actor.CurrentView.LeftExtent + horizontalExtent;
        }

        if(Actor.DestinationPosition.x + horizontalExtent > Actor.CurrentView.RightExtent){
            Actor.DestinationPosition.x = Actor.CurrentView.RightExtent - horizontalExtent;
        }

        if(Actor.DestinationPosition.y - verticalExtent < Actor.CurrentView.BottomExtent){
            Actor.DestinationPosition.y = Actor.CurrentView.BottomExtent + verticalExtent;
        }

        if(Actor.DestinationPosition.y + verticalExtent > Actor.CurrentView.TopExtent){
            Actor.DestinationPosition.y = Actor.CurrentView.TopExtent - verticalExtent;
        }
    }

    public override void ProcessPostFrameState()
    {

    }

}
