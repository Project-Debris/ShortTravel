using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상") ]
    
    [SerializeField] private Player player;
    [SerializeField] private BoxCollider2D bodyCollider; 

    [Header("카메라 수치")]
    [SerializeField] private float viewHeight = 10f;
    [SerializeField] private Vector2 baseOffset = new Vector2(0f, 1.5f);
    [SerializeField] private float lookAhead = 1.5f;

    [SerializeField] private float turnDelay = 0.15f;
    [SerializeField] private float followTime = 0.2f;
    [SerializeField] private float remainRatio = 0.1f;

    private float bodyHeight;
    private Vector2 standingCenterOffset;

    private Camera cam;
    private int facing;
    private float turnTimer;
    private float lookAheadX;


    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

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
        transform.position = Vector3.Lerp(transform.position, TargetPosition(), t);
    }

    public void SnapToTarget()
    {
        facing = 0;
        turnTimer = 0f;
        lookAheadX = 0f;
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

    private Vector3 TargetPosition()
    {
        Vector2 center = bodyCollider.transform.TransformPoint(standingCenterOffset);
        return new Vector3(
            center.x + lookAheadX,
            center.y + baseOffset.y * bodyHeight,
            transform.position.z);
    }
}