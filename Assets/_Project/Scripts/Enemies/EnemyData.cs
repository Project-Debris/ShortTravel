using UnityEngine;

// Create 메뉴에 '적 데이터'를 추가해, 코드 없이 우클릭만으로 새 적 수치를 만들 수 있게 함
// 아래 값은 확정값이 아닌 오류 방지용 임시값
// 기획 2.6.3 적 목록 참고

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

    // 공격 구현 전. 아래 주석 유지.
    // [Header("공격 유형")]
    // public AttackData attack;
}