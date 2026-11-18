using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PinAimTelegraph : MonoBehaviour
{
    [Header("선")]
    [SerializeField] private float maxLength = 20f;
    [SerializeField] private float width = 0.06f;
    [SerializeField] private Color color = new Color(1f, 0.1f, 0.1f, 0.9f);
    [SerializeField] private int sortingOrder = 10;

    [Tooltip("기본적으로 Sprites/Default 사용")]
    [SerializeField] private Material lineMaterial;

    [Tooltip("선이 멈추는 벽 레이어. 비우면 Ground")]
    [SerializeField] private LayerMask wallLayers;

    [Header("방향 고정 후 점멸 (테스트값)")]
    [SerializeField] private float blinkOnTime = 0.1f;
    [SerializeField] private float blinkOffTime = 0.05f;
    [SerializeField] private int blinkCount = 2;
    [SerializeField] private float hideBeforeFire = 0.05f;

    private LineRenderer line;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.startWidth = line.endWidth = width;
        line.startColor = line.endColor = color;
        line.sortingOrder = sortingOrder;
        line.sharedMaterial = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));

        if (wallLayers.value == 0) wallLayers = LayerMask.GetMask("Ground");

        Hide();
    }

    public void ShowTracking(Vector2 origin, Vector2 direction) => Draw(origin, direction);

    public void ShowLocked(Vector2 origin, Vector2 direction, float lockedTime, float timeUntilFire)
    {
        if (timeUntilFire > hideBeforeFire && IsBlinkOn(lockedTime))
            Draw(origin, direction);
        else
            Hide();
    }

    public void Hide() => line.enabled = false;

    private bool IsBlinkOn(float lockedTime)
    {
        float cycle = blinkOnTime + blinkOffTime;
        if (lockedTime >= cycle * blinkCount) return true;

        return lockedTime % cycle < blinkOnTime;
    }

    private void Draw(Vector2 origin, Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        RaycastHit2D wall = Physics2D.Raycast(origin, dir, maxLength, wallLayers);
        Vector2 end = wall ? wall.point : origin + dir * maxLength;

        line.SetPosition(0, origin);
        line.SetPosition(1, end);
        line.enabled = true;
    }
}
