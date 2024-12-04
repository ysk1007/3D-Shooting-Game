using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    public static CasingMemoryPool instance;

    [SerializeField]
    private GameObject casingPrefab;    // 탄피 오브젝트
    private MemoryPool memoryPool;      // 탄피 메모리 풀

    [SerializeField] private Transform casings; // 관리할 부모 오브젝트

    private void Awake()
    {
        instance = this;
        memoryPool = new MemoryPool(casingPrefab);
    }

    public void SpawnCasing(Vector3 position, Vector3 direction)
    {
        GameObject item = memoryPool.ActivatePoolItem();
        item.transform.SetParent(casings);
        item.transform.position = position;
        item.transform.rotation = Random.rotation;
        item.GetComponent<Casing>().Setup(memoryPool, direction);
    }
}
