using System.Collections;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Health playerHealth;
    private Respawn respawn;
    private RepairItemManager repairItemManager;
    private PartSlot partSlot;
    [SerializeField]
    private Room respawnRoom;
    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        respawn = GetComponent<Respawn>();
        repairItemManager = GetComponent<RepairItemManager>();
        partSlot = GetComponent<PartSlot>();

        playerHealth.OnDeath += PlayerDie;
    }

    public void PlayerDie()
    {
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        // 플레이어 조작 비활성화

        // 사망 애니메이션 재생
        yield return new WaitForSeconds(2f);

        respawn.RespawnPlayer();
        playerHealth.HealToMax();
        playerHealth.Revive();
        partSlot.RemovePart();
        repairItemManager.AddMaxItems();
        if (respawnRoom)
        {
            respawnRoom.RespawnEnemies();
        }
    }
}
