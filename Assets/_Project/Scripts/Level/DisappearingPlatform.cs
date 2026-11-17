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

    void Start()
    {

    }

    void Update()
    {

    }
}
