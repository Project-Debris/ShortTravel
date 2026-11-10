using UnityEngine;

public class PartStealer : MonoBehaviour
{
    [Header("탈취")]
    [SerializeField] private float stealRange = 1.5f;
    [SerializeField] private float holdDuration = 0.5f;
    [SerializeField] private LayerMask corpseLayer;

    private Player player;
    private PlayerAttack playerAttack;
    private PlayerHit playerHit;
    private PartSlot partSlot;
    private PlayerInputActions inputActions;

    private PartSteal target;
    private bool isHolding;
    private float holdTimer;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHit = GetComponent<PlayerHit>();
        partSlot = GetComponent<PartSlot>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        if (isHolding)
        {
            Hold();
            return;
        }

        SetTarget(FindNearestCorpse());

        if (inputActions.Player.Interaction.WasPressedThisFrame() && CanStartHold())
            StartHold();
    }

    private bool CanStartHold()
    {
        return target != null
            && player.IsGrounded
            && !playerAttack.IsAttacking
            && !playerHit.IsStunned;
    }

    private void StartHold()
    {
        isHolding = true;
        holdTimer = 0f;
    }

    private void Hold()
    {
        if (ShouldCancelHold())
        {
            isHolding = false;
            return;
        }

        holdTimer += Time.deltaTime;
        if (holdTimer >= holdDuration) CompleteSteal();
    }

    private bool ShouldCancelHold()
    {
        return target == null
            || !target.CanSteal()
            || !inputActions.Player.Interaction.IsPressed()
            || player.MoveInput != 0f
            || playerHit.IsStunned;
    }

    private void CompleteSteal()
    {
        isHolding = false;
        partSlot.Equip(target.PartType, target.MaxUses);
        target.CompleteSteal();
        SetTarget(null);
    }

    private PartSteal FindNearestCorpse()
    {
        PartSteal nearest = null;
        float nearestSqrDistance = float.MaxValue;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, stealRange, corpseLayer))
        {
            if (!hit.TryGetComponent(out PartSteal corpse) || !corpse.CanSteal()) continue;

            float sqrDistance = ((Vector2)(corpse.transform.position - transform.position)).sqrMagnitude;
            if (sqrDistance < nearestSqrDistance)
            {
                nearest = corpse;
                nearestSqrDistance = sqrDistance;
            }
        }

        return nearest;
    }

    private void SetTarget(PartSteal next)
    {
        if (next == target) return;

        if (target != null) target.ShowPrompt(false);
        target = next;
        if (target != null) target.ShowPrompt(true);
    }
}