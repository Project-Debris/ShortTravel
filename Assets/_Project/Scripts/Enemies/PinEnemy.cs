using UnityEngine;

public enum PinEnemyState { Idle, Alert, Aim, Lock, Recover, Stunned, Dead }

public class PinEnemy : Enemy
{
    [Header("대기")]
    [SerializeField] private float lookInterval = 5f;
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private float viewAngle = 60f;
    
    [Header("발견")]
    [SerializeField] private float alertDuration = 0.5f;
    
    [Header("추적 조준")]
    [SerializeField] private float aimDuration = 0.5f;

    private PinEnemyState state;
    private SpriteRenderer spriteRenderer;
    private int blockLayer;
    private float stateTimer;
    private Vector2 aimDirection;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        blockLayer = LayerMask.GetMask("Block");
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        switch (state)
        {
            case PinEnemyState.Idle: Idle(); break;
            case PinEnemyState.Alert: Alert(); break;
            case PinEnemyState.Aim: Aim(); break;
            case PinEnemyState.Lock: Lock(); break;
            case PinEnemyState.Recover: Recover(); break;
            case PinEnemyState.Stunned: Stunned(); break;
            case PinEnemyState.Dead: Dead(); break;
        }
    }

    private void Idle()
    {
        if (stateTimer >= lookInterval)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            stateTimer = 0f;
        }

        Vector2 toPlayer = player.position - transform.position;
        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        if (Vector2.Angle(facing, toPlayer) <= viewAngle * 0.5f && CanSeePlayer(toPlayer))
        {
            aimDirection = toPlayer;
            ChangeState(PinEnemyState.Alert);
        }
    }

    private void Alert()
    {
        Track();
        if (stateTimer >= alertDuration) ChangeState(PinEnemyState.Aim);
    }

    private void Aim()
    {
        Track();
        if (stateTimer >= aimDuration) ChangeState(PinEnemyState.Lock);
    }

    private void Lock() { }
    private void Recover() { }
    private void Stunned() { }
    private void Dead() { }

    private void ChangeState(PinEnemyState next)
    {
        state = next;
        stateTimer = 0f;
    }

    private void Track()
    {
        Vector2 toPlayer = player.position - transform.position;
        if (CanSeePlayer(toPlayer)) aimDirection = toPlayer;

        spriteRenderer.flipX = aimDirection.x < 0;
        Debug.DrawRay(transform.position, aimDirection, Color.blue);
    }

    private bool CanSeePlayer(Vector2 toPlayer)
    {
        return toPlayer.sqrMagnitude <= detectRange * detectRange
            && !Physics2D.Raycast(transform.position, toPlayer, toPlayer.magnitude, blockLayer);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector2 facing = GetComponent<SpriteRenderer>().flipX ? Vector2.left : Vector2.right;

        UnityEditor.Handles.color = new Color(1f, 1f, 0f, 0.2f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, Quaternion.Euler(0, 0, -viewAngle * 0.5f) * facing, viewAngle, detectRange);
    }
#endif
}