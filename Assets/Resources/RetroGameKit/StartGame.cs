using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	void Start () {
        BuildBufferCamera();

        AreaBuilder.BuildArea("demoArea");
	}

    void BuildBufferCamera(){
        GameObject BufferCamera = new GameObject("BufferCamera");
        BufferCamera.AddComponent<Camera>();
        BufferCamera.AddComponent<CameraController>();
        BufferCamera.transform.position = new Vector3(0,0,0);
        EngineProperties.BufferCamera = BufferCamera;
    }
}
