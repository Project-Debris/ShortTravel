using UnityEngine;
using UnityEngine.Serialization;

public class RoomGate : MonoBehaviour
{
    [Header("왼쪽")]
    [SerializeField] private RoomGateSide sideLeft;
    [SerializeField] private Room roomLeft;

    [Header("오른쪽")]
    [SerializeField] private RoomGateSide sideRight;
    [SerializeField] private Room roomRight;

    private RoomGateSide lastTouched;

    public void OnPlayerTouched(RoomGateSide side)
    {
        bool crossed = lastTouched != null && lastTouched != side;
        lastTouched = side;
        if (!crossed) return;

        if (side == sideRight) MoveBetween(roomLeft, roomRight);
        else if (side == sideRight) MoveBetween(roomRight, roomLeft);
    }

    private static void MoveBetween(Room from, Room to)
    {
        if (from != null) from.Exit();
        if (to != null) to.Enter();
    }
}
