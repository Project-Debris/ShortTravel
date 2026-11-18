using UnityEngine;

// RoomGate의 트리거 박스 한쪽. 플레이어가 닿으면 부모 RoomGate에 알린다.
[RequireComponent(typeof(BoxCollider2D))]
public class RoomGateSide : MonoBehaviour
{
    private RoomGate gate;

    private void Awake()
    {
        gate = GetComponentInParent<RoomGate>();
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 콜라이더가 자식에 있어도 Rigidbody가 붙은 루트의 태그로 판별
        Rigidbody2D body = other.attachedRigidbody;
        if (body != null && body.CompareTag("Player")) gate.OnPlayerTouched(this);
    }
}
