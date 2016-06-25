using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerTopViewController : BaseTopViewCharacterController {

    void Start () {
        SpriteWidth = 1f;
        SpriteHeight = 1f;
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

        MovementStateManager.AddState(new PlayerState_TopDownWalk(MovementStateManager));
        MovementStateManager.AddState(new PlayerState_TopDownIdle(MovementStateManager));


        MovementStateManager.SetNextState("PlayerState_TopDownIdle");
    }


}
