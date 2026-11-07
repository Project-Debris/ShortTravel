using UnityEngine;

public enum PinEnemyState { Idle, Alert, Aim, Lock, Recover, Stunned, Dead }

public class PinEnemy : Enemy
{
    private PinEnemyState state;

    // 조정값 ([SerializeField]) - 코드 완성 후 추가 에정.
    [SerializeField] private float lookInterval = 5.00f;
    [SerializeField] private float detectRange = 6.00f; 
    [SerializeField] private float viewAngle = 60f;   // 부채꼴 전체 각도

    // 참조
    private SpriteRenderer spriteRenderer;
    private int blockLayer;


    // 상태 데이터
    float stateTimer; 

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
            case PinEnemyState.Idle:
                Idle(); 
                break;

            case PinEnemyState.Alert:
                Alert();
                break;

            case PinEnemyState.Aim:
                Aim();
                break;

            case PinEnemyState.Lock:
                Lock();
                break;

            case PinEnemyState.Recover:
                Recover();
                break;

            case PinEnemyState.Stunned:
                Stunned();
                break;

            case PinEnemyState.Dead:
                Dead();
                break;

        }
    }

    // 상태별 함수 (UpdateIdle, UpdateAim ...)

    private void Idle()
    {
        if (stateTimer >= lookInterval)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            stateTimer = 0f;
        }

        if (CanSeePlayer())
        {
            ChangeState(PinEnemyState.Alert);
        }
    }

    private void Alert()
    {
        
    }

    private void Aim()
    {
    }

    private void Lock()
    {
    }

    private void Recover()
    {
    }

    private void Stunned()
    {
    }

    private void Dead()
    {
    }

    // 공통 도우미 (ChangeState, CanSeePlayer 

    private void ChangeState(PinEnemyState next)
    {
        state = next;
        stateTimer = 0f;
    }

    private bool CanSeePlayer()
    {
        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.sqrMagnitude > detectRange * detectRange) return false;
        if (Vector2.Angle(spriteRenderer.flipX ? Vector2.left : Vector2.right, toPlayer) > viewAngle * 0.5f) return false;

        return !Physics2D.Raycast(transform.position, toPlayer, toPlayer.magnitude, blockLayer);
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

