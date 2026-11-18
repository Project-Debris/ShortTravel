using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private Color openColor = new Color(1f, 1f, 1f, 0.2f);

    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Open()
    {
        if (IsOpen) return;

        IsOpen = true;
        doorCollider.enabled = false;
        spriteRenderer.color = openColor;
    }
}
