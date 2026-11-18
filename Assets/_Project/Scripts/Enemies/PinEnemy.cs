using UnityEngine;

public enum PinEnemyState { Idle, Alert, Aim, Lock, Recover, Stunned, Dead }

[RequireComponent(typeof(PinLauncher), typeof(PinAimTelegraph))]
public class PinEnemy : Enemy
{
    [Header("1. 대기")]
    [SerializeField] private float lookInterval = 5f;
    [SerializeField] private float detectRange = 6f;
    [SerializeField] private float viewAngle = 150f;

    [Header("2. 발견")]
    [SerializeField] private float alertDuration = 0.5f;
    [Tooltip("발견 반응")]
    [SerializeField] private GameObject alertMark;

    [Header("3. 추적 조준")]
    [SerializeField] private float aimDuration = 0.5f;

    [Header("4. 방향 고정")]
    [SerializeField] private float lockDuration = 1.00f;

    [Header("5. 발사·회복")]
    [SerializeField] private float attackInterval = 1.00f;



    private PinEnemyState state;
    private SpriteRenderer spriteRenderer;
    private int groundLayer;
    private float stateTimer;
    private Vector2 aimDirection;
    private PinLauncher launcher;
    private PinAimTelegraph aimTelegraph;
    private PartSteal partSteal;


    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        launcher = GetComponent<PinLauncher>();
        aimTelegraph = GetComponent<PinAimTelegraph>();
        partSteal = GetComponent<PartSteal>();
        groundLayer = LayerMask.GetMask("Ground");
        SetAlertMark(false);
    }

    private void Update()
    {
        if (state == PinEnemyState.Dead) return;

        if (health.IsDead)
        {
            ChangeState(PinEnemyState.Dead);
            partSteal.BeginSteal();
            return;
        }

        if (state != PinEnemyState.Stunned && hit.IsStunned)
            ChangeState(PinEnemyState.Stunned);

        stateTimer += Time.deltaTime;

        switch (state)
        {
            case PinEnemyState.Idle: Idle(); break;
            case PinEnemyState.Alert: Alert(); break;
            case PinEnemyState.Aim: Aim(); break;
            case PinEnemyState.Lock: Lock(); break;
            case PinEnemyState.Recover: Recover(); break;
            case PinEnemyState.Stunned: Stunned(); break;
        }
    }

    private void Idle()
    {
        if (stateTimer >= lookInterval)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            stateTimer = 0f;
        }

        Vector2 toPlayer = ToPlayer();
        Vector2 facing = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        if (Vector2.Angle(facing, toPlayer) <= viewAngle * 0.5f && CanSeePlayer(toPlayer))
        {
            aimDirection = toPlayer;
            ChangeState(PinEnemyState.Alert);
        }
    }

    private void Alert()
    {
        TrackPlayer();
        if (stateTimer >= alertDuration) ChangeState(PinEnemyState.Aim);
    }

    private void Aim()
    {
        TrackPlayer();
        aimTelegraph.ShowTracking(launcher.FirePosition, aimDirection);

        if (stateTimer >= aimDuration) ChangeState(PinEnemyState.Lock);
    }

    private void Lock()
    {
        aimTelegraph.ShowLocked(launcher.FirePosition, aimDirection, stateTimer, lockDuration - stateTimer);
        if (stateTimer < lockDuration) return;

        launcher.PinAttack(aimDirection);
        ChangeState(PinEnemyState.Recover);
    }

    private void Recover()
    {
        if (stateTimer >= attackInterval) ResumeCombat();
    }

    private void Stunned()
    {
        if (!hit.IsStunned) ResumeCombat();
    }

    private void ChangeState(PinEnemyState next)
    {
        state = next;
        stateTimer = 0f;
        aimTelegraph.Hide();
        SetAlertMark(next == PinEnemyState.Alert);
    }

    private void TrackPlayer()
    {
        Vector2 toPlayer = ToPlayer();
        if (CanSeePlayer(toPlayer)) aimDirection = toPlayer;

        spriteRenderer.flipX = aimDirection.x < 0;
    }

    private Vector2 ToPlayer() => (Vector2)player.position - launcher.FirePosition;

    private bool CanSeePlayer(Vector2 toPlayer)
    {
        return toPlayer.sqrMagnitude <= detectRange * detectRange
            && !Physics2D.Raycast(launcher.FirePosition, toPlayer, toPlayer.magnitude, groundLayer);
    }

    private void ResumeCombat()
    {
        ChangeState(CanSeePlayer(ToPlayer()) ? PinEnemyState.Aim : PinEnemyState.Idle);
    }

    private void SetAlertMark(bool visible)
    {
        if (alertMark != null) alertMark.SetActive(visible);
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
