using UnityEngine;

// 동물의 성향(공격적/ 친근함) 
public enum AnimalType
{
    aggression, 
    friendly
}

// 동물 종류
public enum Animal
{
    bear, // 곰
    wolf, // 늑대
    reindeer, // 순록
    penguin // 펭귄
}

// ScriptableObject로 동물 데이터를 저장하는 클래스 
[CreateAssetMenu(fileName = "Animal", menuName = "New Animal")]
public class AnimalData:ScriptableObject
{
    [Header("Info")]
    public AnimalType animalID; // 동물의 성향 ID
    public Animal animalName; // 동물의 이름
    public float animalHP; // 현재 체력
    public float maxHP; // 최대 체력
    public float animalWalkSpeed; // 걷는 속도
    public int animalRunSpeed; // 달리는 속도
    public float animalPower; // 공격력
    public int animalDetectRange; // 플레이어 탐지 범위
    public float animalAttackRange; // 공격 범위
    public float animalAttackCoolDown; // 공격 쿨타임
    public float wanderRadius; // 탐지 반경
}
