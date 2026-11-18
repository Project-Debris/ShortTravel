using System;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    [Serializable]
    private struct EnemySpawn
    {
        public GameObject prefab;
        public Transform point;
        public bool faceLeft;
    }

    [SerializeField] private string roomId = "room";
    [Tooltip("시작하자마자 진입 처리?")]
    [SerializeField] private bool playerStartsInside = true;
    [SerializeField] private EnemySpawn[] enemySpawns = Array.Empty<EnemySpawn>();
    [SerializeField] private BoxCollider2D cameraBounds;

    private readonly List<GameObject> spawnedEnemies = new List<GameObject>();

    public static Room Current { get; private set; }

    public bool IsPlayerInside { get; private set; }
    public bool TryGetCameraArea(out Bounds area)
    {
        bool valid = cameraBounds != null && cameraBounds.enabled;
        area = valid ? cameraBounds.bounds : default;
        return valid;
    }

    private void Start()
    {
        if (playerStartsInside) Enter();
    }

    private void OnDestroy()
    {
        if (Current == this) Current = null;
    }

    public void Enter()
    {
        if (IsPlayerInside) return;

        IsPlayerInside = true;
        Current = this;
        RespawnEnemies();
        Debug.Log($"[{roomId}] 진입");
    }

    public void Exit()
    {
        if (!IsPlayerInside) return;

        IsPlayerInside = false;
        Debug.Log($"[{roomId}] 퇴장");
    }

    private void RespawnEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null) Destroy(enemy); 
        }
        spawnedEnemies.Clear();

        foreach (EnemySpawn spawn in enemySpawns)
        {
            GameObject enemy = Instantiate(spawn.prefab, spawn.point.position, Quaternion.identity, transform);
            if (enemy.TryGetComponent<SpriteRenderer>(out var sprite)) sprite.flipX = spawn.faceLeft;
            spawnedEnemies.Add(enemy);
        }
    }
}
