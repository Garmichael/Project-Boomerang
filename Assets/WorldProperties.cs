using UnityEngine;
using System.Collections;

public static class WorldProperties {

    public static float Gravity = 1f;
    public static float TerminalVelocity = 20f;
    public static float GlueHeight = 0.3f;
    public static float JumpingGlueHeight = 0.15f;
    public static float TimeScale = 1f;

    public static float GetDeltaTime(){
        return Time.fixedDeltaTime * TimeScale;
    }
}
