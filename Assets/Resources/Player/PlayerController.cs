using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : BaseSideViewCharacterController {

    void Start () {
        SpriteWidth = 1f;
        SpriteHeight = 2f;
        ExtraVerticalRays = 2;
        ExtraHorizontalRays = 3;

        CreateStateInstances();

        base.Start();
    }

    void Update(){
        base.Update();
    }

    void CreateStateInstances()
    {
        MovementStateManager = new StateManager("Player Movement State Manager", this.gameObject);
        CombatStateManager = new StateManager("Player Combat State Manager", this.gameObject);

        MovementStateManager.AddState(new PlayerState_Idle(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_Walk(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_Jump(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_Fall(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_Rotate(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_Shmup(MovementStateManager));

        MovementStateManager.SetNextState("PlayerState_Idle");
    }


}
