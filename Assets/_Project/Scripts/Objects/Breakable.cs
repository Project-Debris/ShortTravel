using UnityEngine;

// 약한 벽·유리 커버 공용. 공격 종류별 필요 타수를 채우면 부서지고, 부서진 상태는 PuzzleState로 유지된다.
//   약한 벽 : 근접 3 / 핀 0(면역). Ground 레이어 → 핀은 기존 코드대로 벽에서 사라짐
//   유리    : 근접 3 / 핀 1.  Default 레이어 → 핀이 유리를 깨고 그대로 날아감
// 핀은 핀 스크립트를 건드리지 않고 OnTriggerEnter2D로 감지한다.
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Breakable : MonoBehaviour, IHittable
{
    [Tooltip("방 안에서 고유한 ID. 비우면 방 재진입 시 다시 생긴다")]
    [SerializeField] private string puzzleId;

    [Header("파괴 타수 (0 = 해당 공격에 면역)")]
    [SerializeField] private int meleeHitsToBreak = 3;
    [SerializeField] private int pinHitsToBreak = 0;

    [Header("피격 반응")]
    [SerializeField] private Color damagedColor = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;
    private int meleeHits;
    private int pinHits;
    private float flashTimer;

    public bool IsBroken { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer.color;

        if (PuzzleState.IsSolved(puzzleId)) Break();
    }

    private void Update()
    {
        if (flashTimer <= 0f) return;

        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0f) ApplyDamageColor();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (PinContact.TryGetSource(other, out var source)) OnHit(source);
    }

    public void OnHit(AttackSource source)
    {
        if (IsBroken) return;

        bool isMelee = source == AttackSource.Melee;
        int hitsToBreak = isMelee ? meleeHitsToBreak : pinHitsToBreak;
        if (hitsToBreak <= 0) return;

        int hitsTaken = isMelee ? ++meleeHits : ++pinHits;
        if (hitsTaken >= hitsToBreak)
        {
            Break();
            return;
        }

        spriteRenderer.color = flashColor;
        flashTimer = flashDuration;
    }

    private void Break()
    {
        IsBroken = true;
        PuzzleState.MarkSolved(puzzleId);
        gameObject.SetActive(false);
    }

    // 남은 타수를 색으로 보여준다: 많이 맞을수록 damagedColor에 가까워짐
    private void ApplyDamageColor()
    {
        float damage = Mathf.Max(Ratio(meleeHits, meleeHitsToBreak), Ratio(pinHits, pinHitsToBreak));
        spriteRenderer.color = Color.Lerp(baseColor, damagedColor, damage);
    }

    private static float Ratio(int hits, int hitsToBreak) => hitsToBreak > 0 ? (float)hits / hitsToBreak : 0f;
}
