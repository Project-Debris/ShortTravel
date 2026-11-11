using UnityEngine;

public enum PartType { None, Pin, Discharge }

public class PartSlot : MonoBehaviour
{
    public PartType CurrentPart { get; private set; }
    public int RemainingUses { get; private set; }

    [Header("핀 파츠")]
    [SerializeField] private PlayerProjectile pinPrefab;
    [SerializeField] private float pinSpeed = 10f;
    [SerializeField] private int pinDamage = 10;

    private PlayerAttack playerAttack;
    private PlayerHit playerHit;
    private Repair repair;
    private PartStealer partStealer;
    private Health health;
    private SpriteRenderer spriteRenderer;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerHit = GetComponent<PlayerHit>();
        repair = GetComponent<Repair>();
        partStealer = GetComponent<PartStealer>();
        health = GetComponent<Health>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        inputActions = new PlayerInputActions();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Update()
    {
        if (inputActions.Player.PartAttack.WasPressedThisFrame() && CanUsePart())
            UsePart();
    }

    public void Equip(PartType part, int maxUses)
    {
        CurrentPart = part;
        RemainingUses = maxUses;
        Debug.Log($"{part} 장착 ({maxUses}회)");
    }

    private bool CanUsePart()
    {
        return CurrentPart != PartType.None
            && RemainingUses > 0
            && !health.IsDead
            && !playerAttack.IsAttacking
            && !playerHit.IsStunned
            && !repair.IsRepairing
            && (partStealer == null || !partStealer.IsHolding);
    }

    private void UsePart()
    {
        switch (CurrentPart)
        {
            case PartType.Pin:
                PlayerProjectile pin = Instantiate(pinPrefab, transform.position, Quaternion.identity);
                pin.Initialize(GetFacingDirection(), pinSpeed, pinDamage);
                break;

            default:
                Debug.Log($"{CurrentPart} 사용 불가");
                return;
        }

        RemainingUses--;
        Debug.Log($"{CurrentPart} 사용 (남은 횟수 {RemainingUses})");

        if (RemainingUses <= 0)
        {
            Debug.Log($"{CurrentPart} 파츠 소진");
            CurrentPart = PartType.None;
        }
    }

    private Vector2 GetFacingDirection()
    {
        return spriteRenderer.flipX ? Vector2.left : Vector2.right;
    }
}