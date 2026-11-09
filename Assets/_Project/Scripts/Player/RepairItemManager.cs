using UnityEngine;
using UnityEngine.InputSystem;

public class RepairItemManager : MonoBehaviour
{
    private Health playerHealth;
    private Repair repair;
    private Player playerMove;
    private PlayerAttack playerAttack;
    private PlayerHit playerHit;

    [Header("아이템 수량")]
    [SerializeField] private int repairItemCount = 0;
    private const int maxRepairItemCount = 3;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        repair = GetComponent<Repair>();
        playerMove = GetComponent<Player>();
        playerAttack = GetComponent<PlayerAttack>();
        playerHit = GetComponent<PlayerHit>();
    }
    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            TryUseRepairItem();
        }
    }

    private void TryUseRepairItem()
    {
        // 아이템이 없거나 이미 수리 중이면 사용 불가
        if (repairItemCount <= 0 || repair.IsRepairing) return;

        // 사망했거나 체력이 가득 찬 경우 사용 불가
        if (playerHealth.IsDead || playerHealth.CurrentHealth >= playerHealth.MaxHealth) return;

        // 공중에 있거나 공격 중이거나 경직 중에는 사용 불가
        if (!playerMove.IsGrounded || playerAttack.IsAttacking || playerHit.IsStunned) return;

        repairItemCount--;
        repair.UseRepairItem();
    }

    public void AddItem(int amount)
    {
        repairItemCount = Mathf.Min(repairItemCount + amount, maxRepairItemCount);
    }
}
