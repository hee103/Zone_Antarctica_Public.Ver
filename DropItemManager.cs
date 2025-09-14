using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    public static DropItemManager Instance; 

    [SerializeField] private GameObject[] dropItemPrefab; // 드랍 가능한 아이템 프리팹 배열
    [SerializeField] private int poolSize = 10; // 각 아이템 풀의 초기 크기

    // 아이템 풀(프리팹 별로 큐를 관리하는 딕셔너리)
    private Dictionary<GameObject, Queue<GameObject>> itemPools = new Dictionary<GameObject, Queue<GameObject>>();

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

        // 각 프리팹마다 풀 생성
        foreach (GameObject prefab in dropItemPrefab)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            // 미리 poolsize만큼 생성해 풀에 저장
            for (int i = 0; i < poolSize; i++)
            {
                GameObject item = Instantiate(prefab);
                item.SetActive(false);

                // 원본 프리팹 정보를 기록하기 위한 objectPooling 컴포넌트 추가
                ObjectPooling pooling = item.AddComponent<ObjectPooling>();
                pooling.originalPrefab = prefab;

                pool.Enqueue(item);
            }

            itemPools[prefab] = pool; // 생성된 풀을 딕셔너리에 저장
        }

    }

    // 특정 위치에 랜점 아이템 3개를 드랍하는 함수
    public GameObject[] DropItem(Vector3 position)
    {
        List<GameObject> droppedItems = new List<GameObject>();

        for (int i = 0; i < 3; i++)
        {
            // 랜덤으로 프리팹 선택
            int randomIndex = Random.Range(0, dropItemPrefab.Length);
            GameObject selectedPrefab = dropItemPrefab[randomIndex];

            // 선택된 프리팹에 맞는 풀 가져오기
            Queue<GameObject> pool = itemPools[selectedPrefab];

            GameObject item;

            // 풀에서 가져오거나 없으면 새로 생성
            if (pool.Count > 0)
            {
                item = pool.Dequeue();
            }
            else
            {
                item = Instantiate(selectedPrefab);
            }
            // 드랍 위치 지정 후 활성화
            item.transform.position = position;
            item.SetActive(true);

            droppedItems.Add(item);
        }
        return droppedItems.ToArray(); // 드랍된 아이템들을 배열로 반환
    }

    // 사용된 아이템을 다시 풀에 반환하는 함수
    public void ReturnItem(GameObject item)
    {
        item.SetActive(false);

        ObjectPooling pooling = item.GetComponent<ObjectPooling>();
        if (pooling != null && pooling.originalPrefab != null && itemPools.ContainsKey(pooling.originalPrefab))
        {
            itemPools[pooling.originalPrefab].Enqueue(item);
        }
    }
}
