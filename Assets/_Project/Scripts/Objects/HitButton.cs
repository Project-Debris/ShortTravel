using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class HitButton : MonoBehaviour, IHittable
{
    [Tooltip("방 안에서 고유한 ID. 비우면 방 재진입 시 다시 잠긴다")]
    [SerializeField] private string puzzleId;
    [SerializeField] private Breakable cover;

    [Header("표시")]
    [SerializeField] private Color idleColor = Color.red;
    [SerializeField] private Color pressedColor = Color.green;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("눌렸을 때 (예: LockedDoor.Open)")]
    [SerializeField] private UnityEvent onPressed;

    private SpriteRenderer spriteRenderer;
    private float flashTimer;

    public bool IsPressed { get; private set; }

    private bool IsCovered => cover != null && !cover.IsBroken;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = idleColor;
    }

    private void Start()
    {
        if (PuzzleState.IsSolved(puzzleId)) Press();
    }

    private void Update()
    {
        if (flashTimer <= 0f) return;

        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f) spriteRenderer.color = CurrentColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PinContact.TryGetSource(other, out var source)) OnHit(source);
    }

    public void OnHit(AttackSource source)
    {
        if (IsCovered) return;

        if (!IsPressed) Press();

        spriteRenderer.color = flashColor;
        flashTimer = flashDuration;
    }

    private void Press()
    {
        IsPressed = true;
        PuzzleState.MarkSolved(puzzleId);
        spriteRenderer.color = CurrentColor;
        onPressed.Invoke();
    }

    private Color CurrentColor => IsPressed ? pressedColor : idleColor;
}
