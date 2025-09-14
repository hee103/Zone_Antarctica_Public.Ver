using UnityEngine;

// 동물을 특정 범위 내에서 랜덤으로 스폰하는 클래스
public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private int spawnCount; // 처음에 스폰할 동물 개수
    [SerializeField] private Vector2 spawnRangeX = new Vector2(0, 0); // X좌표 스폰 범위
    [SerializeField] private Vector2 spawnRangeZ = new Vector2(0, 0); // Z좌표 스폰 범위

    void Start()
    {
        // 시작 시 지정된 개수만큼 동물을 스폰
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomPosition();
        }
    }

    // 랜덤 위치에 동물을 하나 스폰하는 메서드
    void SpawnRandomPosition()
    {
        // 랜덤 X, Z 좌표를 뽑고 Y는 0으로 고정
        Vector3 randomPos = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            0,
            Random.Range(spawnRangeZ.x, spawnRangeZ.y)
        );

        // 풀 매니저에서 랜덤 동물을 가져옴
        GameObject animal = AnimalPoolManager.Instance.GetRandomAnimal();

        // 가져온 동물을 랜덤 위치에 배치
        animal.transform.position = randomPos;

        // AnimalController 컴포넌트를 가져와서 초기화 작업 수행
        AnimalController controller = animal.GetComponent<AnimalController>();
        if (controller != null)
        {
            // HP를 최대 체력으로 리셋
            controller.Data.animalHP = controller.Data.maxHP;

            // 체력 UI 갱신
            controller.RefreshHealthUI();

            // 랜덤한 시작 상태(예: Idle, Walk 등) 설정
            controller.SetRandomStartState();
        }
    }
}
