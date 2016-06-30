using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Camera Camera;
    public GameObject Player;
    public MapViewController CurrentView;

    RenderTexture renderTexture;

    bool RotationMode = false;
    float RotationSpeed = 1f;
    float Rotation = 0f;
    StateManager CameraStateManager;
    public Vector3 DestinationPosition;

	void Start () {
        Player = GameObject.Find("Player");
        Camera = gameObject.GetComponent<Camera>();

        Camera.clearFlags = UnityEngine.CameraClearFlags.Color;
        Camera.backgroundColor = UnityEngine.Color.black;
        Camera.cullingMask = 1 << 0;
        Camera.orthographic = true;
        Camera.nearClipPlane = 0.1f;
        Camera.farClipPlane = 100f;
        Camera.depth = 1f;
        Camera.targetDisplay = 0;


        Camera.aspect = EngineProperties.AspectRatio;
        Camera.orthographicSize = EngineProperties.CameraSize;

        renderTexture = new RenderTexture(EngineProperties.RenderDimensionsWidth, EngineProperties.RenderDimensionsHeight, 24, RenderTextureFormat.Default);
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();
        Camera.targetTexture = renderTexture;

        CameraStateManager = new StateManager("Player Movement State Manager", this.gameObject);

        CameraStateManager.AddState(new CameraState_SnapToPlayer(CameraStateManager));
        CameraStateManager.AddState(new CameraState_Rotation(CameraStateManager));

        CameraStateManager.SetNextState("CameraState_SnapToPlayer");
	}
	
	void Update () {

        CameraStateManager.UpdateState();
        CameraStateManager.ProcessState();
        transform.position = DestinationPosition;
        CameraStateManager.ProcessPostFrameState();
	}

    public void SetCurrentView(MapViewController NewView){
        CurrentView = NewView;
    }

    void OnGUI() {
        if(renderTexture != null){
            GUI.BeginGroup (new Rect (0, 0, Screen.width,Screen.height));
                GUI.DrawTexture(new Rect (0, 0, Screen.width,Screen.height), renderTexture);
            GUI.EndGroup ();
        } else {
            ErrorLogger.Log("Render Texture for Buffer Camera was not created for some reason", "Be careful when editing files in the RetroGameKit source files.");
        }
    }
}


