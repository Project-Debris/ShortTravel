using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ShortTravel/적 데이터")]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;
    public string id = "Enemy.0";

    [Header("능력치")]
    public string attribute;
    public int maxHealth = 3;
    public int attackPower = 1;
}