using System.Collections.Generic;
using UnityEngine;

// 동물 오브젝트 풀을 관리하는 매니저
public class AnimalPoolManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AnimalPoolManager Instance;

    [SerializeField] private GameObject[] animalPrefabs; // 풀링할 동물 프리팹들
    [SerializeField] private int poolSize = 10;          // 각 프리팹당 초기 풀 크기

    // 프리팹별 풀 (각 프리팹마다 Queue 형태로 관리)
    private Dictionary<GameObject, Queue<GameObject>> animalPools = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 각 프리팹별로 풀 생성
        foreach (GameObject prefab in animalPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            // poolSize 만큼 미리 생성해서 비활성화 후 풀에 저장
            for (int i = 0; i < poolSize; i++)
            {
                GameObject animal = Instantiate(prefab);
                animal.SetActive(false);

                // ObjectPooling 컴포넌트를 붙여서 어떤 prefab에서 생성된 것인지 추적
                ObjectPooling pooling = animal.AddComponent<ObjectPooling>();
                pooling.originalPrefab = prefab;

                pool.Enqueue(animal);
            }

            // 해당 프리팹을 key로 해서 풀 등록
            animalPools[prefab] = pool;
        }
    }

    // 랜덤한 동물을 풀에서 꺼내옴
    public GameObject GetRandomAnimal()
    {
        // 랜덤 프리팹 선택
        int randomIndex = Random.Range(0, animalPrefabs.Length);
        GameObject selectedPrefab = animalPrefabs[randomIndex];

        // 해당 프리팹의 풀 가져오기
        Queue<GameObject> pool = animalPools[selectedPrefab];

        GameObject animal;

        // 풀에 오브젝트가 있으면 꺼내고, 없으면 새로 생성
        if (pool.Count > 0)
        {
            animal = pool.Dequeue();
        }
        else
        {
            animal = Instantiate(selectedPrefab);
        }

        // 활성화 후 반환
        animal.SetActive(true);
        return animal;
    }

    // 사용을 마친 동물을 풀에 반환
    public void ReturnAnimal(GameObject animal)
    {
        animal.SetActive(false);

        // ObjectPooling 컴포넌트에서 원본 프리팹을 추적
        ObjectPooling pooling = animal.GetComponent<ObjectPooling>();
        if (pooling != null && pooling.originalPrefab != null && animalPools.ContainsKey(pooling.originalPrefab))
        {
            // 해당 프리팹의 풀에 다시 넣음
            animalPools[pooling.originalPrefab].Enqueue(animal);
        }
    }
}
