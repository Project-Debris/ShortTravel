using System;
using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("컴포넌트")]
    private Collider2D platformCollider;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [Header("시간 설정(초)")]
    [SerializeField] private float disappearDelay = 1.0f;
    [SerializeField] private float reappearDelay = 3.0f;

    [Header("페이드 연출")]
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private bool fadeInOnReappear = true;


    private bool isTriggered = false;

    void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // 접촉면이 위인지 확인
            Vector2 contactNormal = collision.GetContact(0).normal;
            if (contactNormal.y < -0.5f)
            {
                StartCoroutine(DisappearAndReappearRoutine());
            }
        }
    }

    private IEnumerator DisappearAndReappearRoutine()
    {
        isTriggered = true;

        float initialWait = Mathf.Max(0f, disappearDelay - fadeDuration);
        if (initialWait > 0f)
        {
            yield return new WaitForSeconds(initialWait);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        platformCollider.enabled = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(reappearDelay);

        if (fadeInOnReappear && fadeInDuration > 0f)
        {
            elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, originalColor.a, elapsed / fadeInDuration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
        spriteRenderer.color = originalColor;

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;

        isTriggered = false;
    }
}
