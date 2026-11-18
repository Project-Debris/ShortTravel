using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상")]

    [SerializeField] private Player player;
    [SerializeField] private BoxCollider2D bodyCollider;

    [Header("카메라 수치")]
    [SerializeField] private float viewHeight = 10f;
    [SerializeField] private float baseOffsetY = 2.0f;
    [SerializeField] private float lookAhead = 1.0f;

    [SerializeField] private float turnDelay = 0.23f;
    [SerializeField] private float followTime = 0.4f;
    [SerializeField] private float remainRatio = 0.1f;

    [Header("둘러보기")] 
    [SerializeField] private float lookHoldTime = 0.4f;
    [Tooltip("몸 높이(H) 배수")]
    [SerializeField] private float lookUpDistance = 3f;
    [Tooltip("몸 높이(H) 배수")]
    [SerializeField] private float lookDownDistance = 3f;

    [Header("방 전환")]
    [SerializeField] private float roomBlendTime = 0.5f;

    private float bodyHeight;
    private Vector2 standingCenterOffset;

    private Camera cam;
    private int facing;
    private float turnTimer;
    private float lookAheadX;

    private InputAction lookInput;
    private float lookHoldTimer;
    private float lookOffsetY;

    private Room clampRoom;         
    private Bounds blendFrom;       
    private float roomBlendTimer;


    private void Awake()
    {
        cam = GetComponent<Camera>();

        lookInput = new InputAction("Look", InputActionType.Value);
        lookInput.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/downArrow")
            .With("Positive", "<Keyboard>/upArrow");
    }

    private void OnEnable() => lookInput.Enable();
    private void OnDisable() => lookInput.Disable();
    private void OnDestroy() => lookInput.Dispose();

    private void Start()
    {
        bodyHeight = bodyCollider.size.y * bodyCollider.transform.lossyScale.y;
        cam.orthographicSize = viewHeight * bodyHeight * 0.5f;
        SnapToTarget();
    }

    private void LateUpdate()
    {
        float t = 1f - Mathf.Pow(remainRatio, Time.deltaTime / followTime);

        UpdateFacing(player.MoveInput);
        lookAheadX = Mathf.Lerp(lookAheadX, facing * lookAhead * bodyHeight, t);
        lookOffsetY = Mathf.Lerp(lookOffsetY, UpdateLookTarget(), t);
        UpdateRoomBlend();

        transform.position = ClampToRoom(Vector3.Lerp(transform.position, TargetPosition(), t));
    }

    public void SnapToTarget()
    {
        facing = 0;
        turnTimer = 0f;
        lookAheadX = 0f;
        lookHoldTimer = 0f;
        lookOffsetY = 0f;
        clampRoom = Room.Current;        
        roomBlendTimer = roomBlendTime;
        transform.position = TargetPosition();
    }

    private void UpdateFacing(float inputX)
    {
        int input = inputX > 0f ? 1 : inputX < 0f ? -1 : 0;

        if (input == 0 || input == facing)
        {
            turnTimer = 0f;
            return;
        }

        turnTimer += Time.deltaTime;
        if (facing == 0 || turnTimer >= turnDelay)
        {
            facing = input;
            turnTimer = 0f;
        }
    }

    private float UpdateLookTarget()
    {
        float input = lookInput.ReadValue<float>();
        bool canLook = input != 0f && player.IsGrounded && player.MoveInput == 0f;

        lookHoldTimer = canLook ? lookHoldTimer + Time.deltaTime : 0f;
        if (lookHoldTimer < lookHoldTime) return 0f;

        float distance = input > 0f ? lookUpDistance : -lookDownDistance;
        return distance * bodyHeight;
    }

    private Vector3 TargetPosition()
    {
        Vector2 center = bodyCollider.transform.TransformPoint(standingCenterOffset);
        return ClampToRoom(new Vector3(
            center.x + lookAheadX,
            center.y + baseOffsetY * bodyHeight + lookOffsetY,
            transform.position.z));
    }

    private void UpdateRoomBlend()
    {
        Room room = Room.Current;
        if (room != clampRoom)
        {
            bool isFirstRoom = clampRoom == null;
            clampRoom = room;
            blendFrom = new Bounds(transform.position, ViewSize());
            roomBlendTimer = isFirstRoom ? roomBlendTime : 0f;
        }

        roomBlendTimer += Time.deltaTime;
    }


    private Vector3 ClampToRoom(Vector3 position)
    {
        if (!TryGetActiveArea(out Bounds area)) return position;

        Vector3 half = ViewSize() * 0.5f;
        position.x = ClampAxis(position.x, area.min.x + half.x, area.max.x - half.x, area.center.x);
        position.y = ClampAxis(position.y, area.min.y + half.y, area.max.y - half.y, area.center.y);
        return position;
    }

    private bool TryGetActiveArea(out Bounds area)
    {
        Room room = Room.Current;
        if (room == null || !room.TryGetCameraArea(out area))
        {
            area = default;
            return false;
        }

        if (roomBlendTime <= 0f || roomBlendTimer >= roomBlendTime) return true;

        float s = Mathf.SmoothStep(0f, 1f, roomBlendTimer / roomBlendTime);
        area = new Bounds(
            Vector3.Lerp(blendFrom.center, area.center, s),
            Vector3.Lerp(blendFrom.size, area.size, s));
        return true;
    }

    private Vector3 ViewSize()
    {
        float height = cam.orthographicSize * 2f;
        return new Vector3(height * cam.aspect, height, 0f);
    }

    private static float ClampAxis(float value, float min, float max, float center) =>
        min > max ? center : Mathf.Clamp(value, min, max);
}
