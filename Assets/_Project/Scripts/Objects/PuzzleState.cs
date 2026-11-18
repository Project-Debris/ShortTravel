using System.Collections.Generic;
using UnityEngine;

// 방 재진입·사망 후에도 유지할 퍼즐 상태 (부서진 벽, 깨진 유리, 눌린 버튼).
// 지금은 현재 플레이 동안만 유지한다. 저장 시스템이 생기면 이 목록을 저장·복원하면 된다.
public static class PuzzleState
{
    private static readonly HashSet<string> solved = new HashSet<string>();

    public static bool IsSolved(string id) => !string.IsNullOrEmpty(id) && solved.Contains(id);

    public static void MarkSolved(string id)
    {
        if (!string.IsNullOrEmpty(id)) solved.Add(id);
    }

    // Enter Play Mode 옵션에서 도메인 리로드를 꺼도 이전 플레이 상태가 남지 않게 한다.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay() => solved.Clear();
}
