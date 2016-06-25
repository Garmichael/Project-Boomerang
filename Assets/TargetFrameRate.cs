using UnityEngine;
using System.Collections;

public class TargetFrameRate : MonoBehaviour {
    public int Target = 10;


    void Start () {
//        QualitySettings.vSyncCount = 0;
    }

    void Update(){
//        if(Application.targetFrameRate != Target){
//            Application.targetFrameRate = Target;
//        }
    }

    private void OnGUI(){
        GUI.Label(new Rect(0, 100, 200, 50), new GUIContent("FPS: " + 1 / Time.deltaTime));
    }

}