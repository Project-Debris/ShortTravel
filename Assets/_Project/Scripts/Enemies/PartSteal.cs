using System.Collections;
using UnityEngine;

public class PartSteal : MonoBehaviour
{
    [Header("파츠")]
    [SerializeField] private PartType partType = PartType.Pin;
    [SerializeField] private int maxUses = 3;

    [Header("사망 전환")]
    [SerializeField] private float deathTransitionDuration = 0.5f;
    [SerializeField] private Color deadTint = new Color(0.35f, 0.35f, 0.35f, 1f);

    [Header("표시")]
    [SerializeField] private GameObject glow;
    [SerializeField] private GameObject prompt;

    private bool isStealable;
    private SpriteRenderer spriteRenderer;
    private EnemyAttack enemyAttack;

    public PartType PartType => partType;
    public int MaxUses => maxUses;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAttack = GetComponent<EnemyAttack>();

        SetGlow(false);
        SetPrompt(false);
    }

    public void BeginSteal()
    {
        StopAttacking();
        BecomePassable();
        StartCoroutine(EnableStealAfterTransition());
    }

    public bool CanSteal() => isStealable;

    public void ShowPrompt(bool visible) => SetPrompt(visible);

    public void CompleteSteal()
    {
        isStealable = false;
        SetGlow(false);
        SetPrompt(false);
    }

    private void StopAttacking()
    {
        if (enemyAttack == null) return;

        enemyAttack.CancelAttack(true);
        enemyAttack.enabled = false;
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
        Color startColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        Color endColor = startColor * deadTint;

        float timer = 0f;
        while (timer < deathTransitionDuration)
        {
            timer += Time.deltaTime;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.Lerp(startColor, endColor, timer / deathTransitionDuration);
            yield return null;
        }

        if (spriteRenderer != null) spriteRenderer.color = endColor;

        isStealable = true;
        SetGlow(true);
    }

    private void SetGlow(bool active)
    {
        if (glow != null) glow.SetActive(active);
    }

    private void SetPrompt(bool active)
    {
        if (prompt != null) prompt.SetActive(active);
    }
}
