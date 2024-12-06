using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ItemMemoryPool : MonoBehaviour
{
    public static ItemMemoryPool instance;

    [SerializeField] private GameObject[] itemPrefab;      // 아이템 프리팹

    private MemoryPool[] itemPool;                         // 아이템 메모리 풀

    [SerializeField] private Transform items;              // 관리할 부모 오브젝트

    private void Awake()
    {
        instance = this;

        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        itemPool = new MemoryPool[itemPrefab.Length];
        for (int i = 0; i < itemPrefab.Length; ++i)
        {
            itemPool[i] = new MemoryPool(itemPrefab[i],items);
        }
    }

    private void Start()
    {
        SpawnItem(new Vector3(0, 0.3f, 3),2);
        SpawnItem(new Vector3(0, 0.3f, 3), 2);
        SpawnItem(new Vector3(0, 0.3f, 3), 2);
        SpawnItem(new Vector3(2, 0.3f, 3), 1);
        SpawnItem(new Vector3(-2, 0.3f, 3), 0);
    }

    public void SpawnItem(Vector3 position, int i)
    {
        GameObject item = itemPool[i].ActivatePoolItem();
        item.transform.SetParent(items);
        item.transform.position = position;
        item.GetComponent<ItemBase>().SetUp(itemPool[i]);
        //item.GetComponent<Bullet>().Setup(this, bulletPool[(int)type], impactPool[(int)type]);
    }
}
