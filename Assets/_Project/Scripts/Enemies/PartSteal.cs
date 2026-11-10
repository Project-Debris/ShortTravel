using System.Collections;
using UnityEngine;

public class PartSteal : MonoBehaviour
{
    [Header("파츠")]
    [SerializeField] private PartType partType = PartType.Pin;
    [SerializeField] private int maxUses = 3;

    [Header("사망 전환")]
    [SerializeField] private float deathTransitionDuration = 0.5f;

    [Header("표시")]
    [SerializeField] private GameObject glow;
    [SerializeField] private GameObject prompt;

    private bool isStealable;

    public PartType PartType => partType;
    public int MaxUses => maxUses;

    private void Awake()
    {
        glow.SetActive(false);
        prompt.SetActive(false);
    }

    public void BeginSteal()
    {
        BecomePassable();
        StartCoroutine(EnableStealAfterTransition());
    }

    public bool CanSteal() => isStealable;

    public void ShowPrompt(bool visible) => prompt.SetActive(visible);

    public void CompleteSteal()
    {
        isStealable = false;
        glow.SetActive(false);
        prompt.SetActive(false);
    }

    private void BecomePassable()
    {
        GetComponent<Collider2D>().isTrigger = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private IEnumerator EnableStealAfterTransition()
    {
        yield return new WaitForSeconds(deathTransitionDuration);
        isStealable = true;
        glow.SetActive(true);
    }
}