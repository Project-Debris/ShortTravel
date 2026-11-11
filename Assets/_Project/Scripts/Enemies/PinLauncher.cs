using UnityEngine;

public class PinLauncher : MonoBehaviour
{
    [Header("핀")]
    [SerializeField] private Projectile pinPrefab;
    [SerializeField] private float pinSpeed = 10f;
    [SerializeField] private int pinDamage = 10;

    [Header("발사 위치 (비우면 몸 중심)")]
    [SerializeField] private Transform firePoint;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
    }

    public void PinAttack(Vector2 direction)
    {
        Projectile pin = Instantiate(pinPrefab, firePoint.position, Quaternion.identity);
        pin.Initialize(direction, pinSpeed, pinDamage);
    }
}