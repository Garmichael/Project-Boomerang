using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using System.IO;
#endif

public class BaseSideViewCharacterController : MonoBehaviour {

    protected StateManager MovementStateManager;
    protected StateManager CombatStateManager;

    protected float SpriteWidth = 1f;
    protected float SpriteHeight = 2f;

    protected float Gravity = WorldProperties.Gravity;

    protected float RoundingPointPrecision = 100f;

    [Header("Jumping")]
    public float JumpStrength = 10f;
    public float MaximumJumpDuration = 0.25f;
    public bool CanChangeDirectionInMidAir = false;

    [Header("Ground Movement")]
    public float MaximumGroundSpeed = 10f;
    public float GroundAcceleration = 0.5f;
    public float GroundDeceleration = 2f;

    [Header("Air Movement Forward")]
    public float MaximumAirSpeedForward = 10f;
    public float AirAccelerationForward = 0.5f;
    public float AirDecelerationForward = 0.1f;

    [Header("Air Movement Backward")]
    public float MaximumAirSpeedBackward = 10f;
    public float AirAccelerationBackward = 0.5f;
    public float AirDecelerationBackward = 0.1f;

    [Header("Miscelaneous")]
    public bool CollidesWithGeometry = true;

    [Header("Dynamic Properties")]
    public string MovementState;
    public string CombatState;

    public bool FacingRight = true;
    public bool IsGrounded = false;

    public Vector3 Velocity = new Vector3(0, 0, 0);

    public GameObject Sprite;
    protected GameObject RayOriginTopLeft;
    protected GameObject RayOriginTopRight;
    protected GameObject RayOriginBottomRight;
    protected GameObject RayOriginBottomLeft;

    protected float HorizontalRayInset = 0.05f;
    protected float VerticalRayInset = 0.25f;

    protected int ExtraHorizontalRays = 1;
    protected int ExtraVerticalRays = 0;

    protected BoxCollider Collider;

    public float MaxOffsetUp;
    public float MaxOffsetRight;
    public float MaxOffsetDown;
    public float MaxOffsetLeft;

    protected void Start () {
        Sprite = GameObject.Find("Sprite");
        PlaceRayOrigins();
        BuildBoundingBox();
	}

	protected void Update () {
        if(FacingRight){
            Sprite.transform.localScale = new Vector3(1,1,1);
        } else {
            Sprite.transform.localScale = new Vector3(-1,1,1);
        }

        MovementStateManager.UpdateState();
        GenerateMaxOffsets();
        SetIsGrounded();
        ProcessStates();
        ApplyVelocity();
        ProcessPostFrameStates();
	}

    void PlaceRayOrigins()
    {
        float HorizontalOffset = SpriteWidth / 2;
        float VerticalOffset = SpriteHeight / 2;

        RayOriginTopLeft = new GameObject();
        RayOriginTopLeft.name = "RayOriginTopLeft";
        RayOriginTopLeft.transform.parent = transform;
        RayOriginTopLeft.transform.localPosition = new Vector3(-HorizontalOffset + HorizontalRayInset, VerticalOffset - VerticalRayInset, 0);

        RayOriginTopRight = new GameObject();
        RayOriginTopRight.name = "RayOriginTopRight";
        RayOriginTopRight.transform.parent = transform;
        RayOriginTopRight.transform.localPosition = new Vector3(HorizontalOffset - HorizontalRayInset, VerticalOffset - VerticalRayInset, 0);

        RayOriginBottomRight = new GameObject();
        RayOriginBottomRight.name = "RayOriginBottomRight";
        RayOriginBottomRight.transform.parent = transform;
        RayOriginBottomRight.transform.localPosition = new Vector3(HorizontalOffset - HorizontalRayInset, -VerticalOffset + VerticalRayInset, 0);

        RayOriginBottomLeft = new GameObject();
        RayOriginBottomLeft.name = "RayOriginBottomLeft";
        RayOriginBottomLeft.transform.parent = transform;
        RayOriginBottomLeft.transform.localPosition = new Vector3(-HorizontalOffset + HorizontalRayInset, -VerticalOffset + VerticalRayInset, 0);
    }

    void BuildBoundingBox()
    {
        gameObject.AddComponent<BoxCollider>();
        Collider = gameObject.GetComponent<BoxCollider>();
        Collider.size = new Vector3(SpriteWidth, SpriteHeight, 1);
    }

    void GenerateMaxOffsets()
    {
        MaxOffsetUp = GenerateMaxOffset("UP");
        MaxOffsetRight = GenerateMaxOffset("RIGHT");
        MaxOffsetDown = GenerateMaxOffset("DOWN");
        MaxOffsetLeft = GenerateMaxOffset("LEFT");
    }

    public float GenerateMaxOffset(string Direction)
    {
        Vector3 RayDirection = new Vector3(0, 0, 0);
        RaycastHit Hit;

        float HitDistance = 1000f;
        float ShortestDistance = 1000f;
        float AddedOffset = 0f;

        RoundOffPositionFloat();

        if(!CollidesWithGeometry){
            return HitDistance;
        }

        List<Vector3> OriginList = new List<Vector3>();
        OriginList.Clear();

        Direction = Direction.ToLower();

        if (Direction == "up")
        {
            RayDirection = new Vector3(0,1,0);
            AddedOffset = VerticalRayInset;
            OriginList.Add(RayOriginTopLeft.transform.position);
            OriginList.Add(RayOriginTopRight.transform.position);
        }

        if (Direction == "right")
        {
            AddedOffset = HorizontalRayInset;
            RayDirection = new Vector3(1,0,0);
            OriginList.Add(RayOriginTopRight.transform.position);
            OriginList.Add(RayOriginBottomRight.transform.position);
        }

        if (Direction == "down")
        {
            AddedOffset = VerticalRayInset;
            RayDirection = new Vector3(0,-1,0);
            OriginList.Add(RayOriginBottomLeft.transform.position);
            OriginList.Add(RayOriginBottomRight.transform.position);
        }

        if (Direction == "left")
        {
            AddedOffset = HorizontalRayInset;
            RayDirection = new Vector3(-1,0,0);
            OriginList.Add(RayOriginTopLeft.transform.position);
            OriginList.Add(RayOriginBottomLeft.transform.position);
        }

        if((Direction == "left" || Direction == "right") && ExtraHorizontalRays > 0){
            float NewPointSpacing = (OriginList.ElementAt(0).y - OriginList.ElementAt(1).y) / (ExtraHorizontalRays + 1);
            for(int i = 1; i <= ExtraHorizontalRays; i++){
                OriginList.Add(OriginList.ElementAt(0) - new Vector3(0, NewPointSpacing * i, 0));
            }
        }

        if((Direction == "up" || Direction == "down") && ExtraVerticalRays > 0){
            float NewPointSpacing = (OriginList.ElementAt(0).x - OriginList.ElementAt(1).x) / (ExtraVerticalRays + 1);
            for(int i = 1; i <= ExtraVerticalRays; i++){
                OriginList.Add(OriginList.ElementAt(0) - new Vector3(NewPointSpacing * i, 0, 0));
            }
        }


        foreach (Vector3 OriginPoint in OriginList){
            bool HitSolid;
            bool HitSolidOnTop;

            #if UNITY_EDITOR
            Debug.DrawRay(OriginPoint, RayDirection, Color.magenta);
            #endif

            if(Physics.Raycast(new Ray(OriginPoint, RayDirection), out Hit, HitDistance)){
                HitSolid = Hit.transform.gameObject.layer == LayerMask.NameToLayer("Solid");
                HitSolidOnTop = Hit.transform.gameObject.layer == LayerMask.NameToLayer("SolidOnTop") && Direction == "down";

                if((HitSolid || HitSolidOnTop) && Hit.distance < ShortestDistance){
                    ShortestDistance = Hit.distance;
                }
            }
        }

        ShortestDistance -= AddedOffset;
        ShortestDistance = Mathf.Round(ShortestDistance * RoundingPointPrecision) / RoundingPointPrecision;

        return ShortestDistance;
    }

    void SetIsGrounded(){

        IsGrounded = MaxOffsetDown == 0;

        if(IsGrounded){
            Velocity.y = 0;
        }

    }

    protected void ProcessStates(){
        if(MovementStateManager != null){
            MovementStateManager.ProcessState();
            MovementState = MovementStateManager.GetCurrentSate().Name;
        }

        if(CombatStateManager != null){
            CombatStateManager.ProcessState();
            CombatState = MovementStateManager.GetCurrentSate().Name;
        }
    }

    void ApplyVelocity(){

        ApplyHorizontalVelocity();
        GenerateMaxOffsets();

        ApplyVerticalVelocity();
        GenerateMaxOffsets();
    }

    void ApplyVerticalVelocity(){
        float NewYDistance;

        NewYDistance = Velocity.y * WorldProperties.GetDeltaTime();

        NewYDistance = ConstrainVerticalVelocity(NewYDistance);

        transform.position += new Vector3(0, NewYDistance, 0);
    }

    void ApplyHorizontalVelocity(){
        float NewXDistance;

        if(IsGrounded){
            if(FacingRight && Velocity.x > MaximumGroundSpeed){
                Velocity.x = MaximumGroundSpeed;
            }

            if(!FacingRight && Velocity.x < -MaximumGroundSpeed){
                Velocity.x = -MaximumGroundSpeed;
            }
        } else {
            if(FacingRight && Velocity.x > MaximumAirSpeedForward){
                Velocity.x = MaximumAirSpeedForward;
            }

            if(FacingRight && Velocity.x < -MaximumAirSpeedBackward){
                Velocity.x = -MaximumAirSpeedBackward;
            }

            if(!FacingRight && Velocity.x < -MaximumAirSpeedForward){
                Velocity.x = -MaximumAirSpeedForward;
            }

            if(!FacingRight && Velocity.x > MaximumAirSpeedBackward){
                Velocity.x = MaximumAirSpeedBackward;
            }

        }

        NewXDistance = Velocity.x * WorldProperties.GetDeltaTime();

        NewXDistance = ConstrainHorizontalVelocity(NewXDistance);

        transform.position += new Vector3(NewXDistance, 0, 0);
    }

    float ConstrainVerticalVelocity(float distance){

        if(distance > 0 && distance >= MaxOffsetUp){
            distance = MaxOffsetUp;
        } else if(distance < 0 && -distance >= MaxOffsetDown){
            distance = -MaxOffsetDown;
        }

        return distance;
    }

    float ConstrainHorizontalVelocity(float distance){
        if(distance > 0f && distance >= MaxOffsetRight){
            distance = MaxOffsetRight;
            Velocity.x = 0f;

        } else if(distance < 0 && -distance >= MaxOffsetLeft){
            distance = -MaxOffsetLeft;
            Velocity.x = 0;
        }

        return distance;
    }

    void RoundOffPositionFloat(){
        transform.position = new Vector3(
                Mathf.Round(transform.position.x * RoundingPointPrecision) / RoundingPointPrecision,
                Mathf.Round(transform.position.y * RoundingPointPrecision) / RoundingPointPrecision,
                Mathf.Round(transform.position.z * RoundingPointPrecision) / RoundingPointPrecision
        );
    }

    void ProcessPostFrameStates(){
        MovementStateManager.ProcessPostFrameState();
        CombatStateManager.ProcessPostFrameState();
    }
}
