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
        if (IsPlayer(other)) gate.OnPlayerEnter(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other)) gate.OnPlayerExit(this);
    }

    private static bool IsPlayer(Collider2D other)
    {
        Rigidbody2D body = other.attachedRigidbody;
        return body != null && body.CompareTag("Player");
    }
}
