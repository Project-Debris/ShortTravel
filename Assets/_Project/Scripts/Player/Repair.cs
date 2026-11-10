using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


public class Repair : MonoBehaviour
{
    [Header("컴포넌트")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private TMP_Text repairProgressText;

    [Header("수리아이템 설정")]
    [SerializeField] private float repairDuration = 1f;
    [SerializeField] private float healAmount = 40f;
    private Coroutine repairCoroutine;
    public bool IsRepairing { get; private set; } = false;//수리진행중인지 읽어서 이동, 점프, 공격 불가하게 해주세요

    public void UseRepairItem()
    {
        IsRepairing = true;

        repairProgressText.gameObject.SetActive(true);

        repairCoroutine = StartCoroutine(RepairRoutine());
    }

    private IEnumerator RepairRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < repairDuration)
        {
            elapsedTime += Time.deltaTime;

            repairProgressText.text = $"{Mathf.Clamp01(elapsedTime / repairDuration) * 100f:F0}%";

            yield return null;
        }

        playerHealth.Heal(healAmount);

        IsRepairing = false;
        repairCoroutine = null;

        yield return new WaitForSeconds(0.5f);


        repairProgressText.gameObject.SetActive(false);
    }

    public void CancelUse() //피격 등으로 수리 취소 시 호출
    {
        if (!IsRepairing) return;

        StopCoroutine(repairCoroutine);

        IsRepairing = false;
        repairCoroutine = null;

        repairProgressText.gameObject.SetActive(false);
    }
}
