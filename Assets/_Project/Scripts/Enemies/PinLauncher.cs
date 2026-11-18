using UnityEngine;

public class PinLauncher : MonoBehaviour
{
    [Header("핀")]
    [SerializeField] private Projectile pinPrefab;
    [SerializeField] private float pinSpeed = 30f;
    [SerializeField] private int pinDamage = 10;

    [Header("발사 위치 (비우면 몸 중심)")]
    [SerializeField] public Transform firePoint;

    private int wallLayer;

    public Vector2 FirePosition
    {
        get
        {
            Vector2 body = transform.position;
            Vector2 muzzle = firePoint.position;
            return Physics2D.Linecast(body, muzzle, wallLayer) ? body : muzzle;
        }
    }

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
        wallLayer = LayerMask.GetMask("Ground");
    }

    public void PinAttack(Vector2 direction)
    {
        Projectile pin = Instantiate(pinPrefab, FirePosition, Quaternion.identity);
        pin.Initialize(direction, pinSpeed, pinDamage);
    }
}