using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasingMemoryPool : MonoBehaviour
{
    public static CasingMemoryPool instance;

    [SerializeField]
    private GameObject casingPrefab;    // ź�� ������Ʈ
    private MemoryPool memoryPool;      // ź�� �޸� Ǯ

    [SerializeField] private Transform casings; // ������ �θ� ������Ʈ

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
