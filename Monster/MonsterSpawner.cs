using UnityEngine;

// 몬스터를 특정 범위 내에서 랜덤으로 스폰하는 클래스
public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private int spawnCount; // 스폰할 몬스터 수
    [SerializeField] private Vector2 spawnRangeX = new Vector2(0, 0); // x축 스폰 범위
    [SerializeField] private Vector2 spawnRangeZ = new Vector2(0, 0); // z축 스폰 범위

    // 게임 시작 시 몬스터 스폰
    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnRandomPosition(); // 랜덤 위치에 몬스터 스폰
        }
    }

    // 랜덤 위치에 몬스터를 스폰하는 함수
    void SpawnRandomPosition()
    {
        // x,z 좌표를 랜덤하게 설정, y는 0으로 고정
        Vector3 randomPos = new Vector3(
            Random.Range(spawnRangeX.x, spawnRangeX.y),
            0,
            Random.Range(spawnRangeZ.x, spawnRangeZ.y)
        );

        // MonsterPoolManager에서 랜덤한 몬스터 가져오기
        GameObject monster = MonsterPoolManager.Instance.GetRandomMonster();

        // 스폰 위치 지정
        monster.transform.position = randomPos;

        // 몬스터 컨트롤러 초기화
        MonsterController controller = monster.GetComponent<MonsterController>();
        if (controller != null)
        {
            controller.Data.monsterHP = controller.Data.maxHP; // 체력 초기화
            controller.RefreshHealthUI(); // 체력 UI 갱신
            controller.SetRandomStartState(); // 랜덤 초기 상태 설정
        }
    }
}
