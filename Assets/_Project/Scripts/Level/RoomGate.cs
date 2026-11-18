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

    private bool playerInLeft;
    private bool playerInRight;

    public void OnPlayerEnter(RoomGateSide side) => SetPlayerIn(side, true);

    public void OnPlayerExit(RoomGateSide side)
    {
        SetPlayerIn(side, false);
        if (playerInLeft || playerInRight) return;

        if (side == sideLeft) MoveInto(roomLeft, roomRight);
        else if (side == sideRight) MoveInto(roomRight, roomLeft);
    }

    private void SetPlayerIn(RoomGateSide side, bool isIn)
    {
        if (side == sideLeft) playerInLeft = isIn;
        else if (side == sideRight) playerInRight = isIn;
    }

    private static void MoveInto(Room to, Room from)
    {
        if (to != null && to.IsPlayerInside) return;

        if (from != null) from.Exit();
        if (to != null) to.Enter();
    }

}
