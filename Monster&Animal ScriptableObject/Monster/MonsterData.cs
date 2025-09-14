using UnityEngine;

// 몬스터 종류를 구분하는 열거형
public enum MonsterType
{
    Monster1_Base, // 기본형 몬스터
    Monster2_lie, // 누워있는 몬스터 
    Monster3_Throw // 투척 공격형 몬스터
}

// 몬스터의 스탯 및 속성을 ScriptableObject로 관리하는 클래스
[CreateAssetMenu(fileName = "Monster", menuName = "New Monster")]
public class MonsterData : ScriptableObject
{
    [Header("Info")]
    public MonsterType monsterID; // 몬스터 ID
    public float monsterHP; // 현재 체력
    public float maxHP; // 최대 체력
    public int monsterPower; // 공격력
    public int monsterWalkSpeed; // 걷는 속도
    public int monsterRunSpeed; // 달리는 속도
    public float monsterAttackRange; // 공격 범위
    public int monsterDetectRange; // 탐지 범위
    public float wanderRadius; // 탐지 반경
    public float attackCoolDown; // 공격 쿨타임
   
}
