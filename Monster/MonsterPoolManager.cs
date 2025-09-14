using System.Collections.Generic;
using UnityEngine;

// 몬스터 객체 풀을 관리하는 매니저로 몬스터를 미리 생성해두고 재사용하여 성능 최적화
public class MonsterPoolManager : MonoBehaviour
{
    public static MonsterPoolManager Instance; 

    [SerializeField] private GameObject[] monsterPrefabs; // 풀에 사용할 몬스터 프리팹 배열
    [SerializeField] private int poolSize = 10; // 각 몬스터 풀 초기 생성 개수

    // 프리팹별로 몬스터 큐를 관리하는 딕셔너리
    private Dictionary<GameObject, Queue<GameObject>> monsterPools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 각 몬스터 프리팹별로 큐 생성 및 초기화
        foreach (GameObject prefab in monsterPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject monster = Instantiate(prefab); // 몬스터 인스턴스 생성
                monster.SetActive(false); // 비황성화 상태로 대기
                ObjectPooling pooling = monster.AddComponent<ObjectPooling>(); // 풀 정보 저장
                pooling.originalPrefab = prefab; // 원본 프리팹 연결

                pool.Enqueue(monster); // 큐에 추가
            }

            monsterPools[prefab] = pool; // 딕셔너리에 풀 등록
        }
    }

    // 랜덤한 몬스터 풀에서 몬스터 가져오기
    public GameObject GetRandomMonster()
    {
        int randomIndex = Random.Range(0, monsterPrefabs.Length);
        GameObject selectedPrefab = monsterPrefabs[randomIndex];

        Queue<GameObject> pool = monsterPools[selectedPrefab];

        GameObject monster;

        // 큐에 남은 몬스터가 있으면 가져오고 없으면 새로 생성
        if (pool.Count > 0)
        {
            monster = pool.Dequeue();
        }
        else
        {
            monster = Instantiate(selectedPrefab);
        }

        monster.SetActive(true);
        return monster;
    }

    // 사용한 몬스터를 풀에 반환
    public void ReturnMonster(GameObject monster)
    {
        monster.SetActive(false); // 비활성화

        ObjectPooling pooling = monster.GetComponent<ObjectPooling>();
        if (pooling != null && pooling.originalPrefab != null && monsterPools.ContainsKey(pooling.originalPrefab))
        {
            monsterPools[pooling.originalPrefab].Enqueue(monster); // 원래 큐에 반환
        }

        //튜토리얼 완료 처리
        UITutorialGuide guide = GameManager.Instance.UiTutorialGuide;
        TutorialData tutorialData = guide.GetTutorialData();

        if (guide == null)
            return;

        if (tutorialData == null || tutorialData.tutorialType != TutorialType.Combat)
            return;

        if (guide.IsObjectiveIncomplete(TutorialAction.KillMonster))
        {
            guide.SetObjectiveComplete(TutorialAction.KillMonster);
        }
    }
}