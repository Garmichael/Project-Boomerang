using UnityEngine;
using System.Collections;

public class MapViewController : MonoBehaviour {
    public float TopExtent;
    public float RightExtent;
    public float BottomExtent;
    public float LeftExtent;

    GameObject Player;

    CameraController cameraController;

	void Start () {
        Player = GameObject.Find("Player");

        cameraController = EngineProperties.BufferCamera.GetComponent<CameraController>();

        gameObject.layer = LayerMask.NameToLayer("MapView");
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        Destroy(gameObject.GetComponent<MeshRenderer>());

        TopExtent = gameObject.transform.position.y + gameObject.transform.localScale.y / 2f;
        RightExtent = gameObject.transform.position.x + gameObject.transform.localScale.x / 2f;
        BottomExtent = gameObject.transform.position.y - gameObject.transform.localScale.y / 2f;
        LeftExtent = gameObject.transform.position.x - gameObject.transform.localScale.x / 2f;
	}

	void Update () {
        if(this.gameObject.GetComponent<BoxCollider>().bounds.Contains(Player.transform.position))
        {
            cameraController.SetCurrentView(this);
        }
	}
}
