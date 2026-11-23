using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Vector3 spawnPosition;
    public void RespawnPlayer()
    {
        transform.position = spawnPosition;
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        spawnPosition = newSpawnPoint;
    }
}
