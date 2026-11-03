using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상") ]
    [SerializeField] private Transform target ; 
    [SerializeField] private Player player; 

    [Header("카메라 수치")]
    [SerializeField] private float bodyHeight = 1.5f;
    [SerializeField] private float viewHeight = 10f;
    [SerializeField] private Vector2 baseOffset = new Vector2(0f, 1.5f);
    [SerializeField] private float lookAhead = 1.5f;

    [SerializeField] private float turnDelay = 0.15f;
    [SerializeField] private float followTime = 0.2f;
    [SerializeField] private float remainRatio = 0.1f;

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
        SnapToTarget();
    }

    private void LateUpdate()
    {
        float t = 1f - Mathf.Pow(remainRatio, Time.deltaTime / followTime);

        cam.orthographicSize = viewHeight * bodyHeight * 0.5f;

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
        Vector2 offset = baseOffset * bodyHeight + new Vector2(lookAheadX, 0f);
        return new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
    }
}