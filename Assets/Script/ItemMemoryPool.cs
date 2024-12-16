using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum ItemType { HealthItem = 0, MagazineItem = 1, DropGun = 2}

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

    }

    public void SpawnItem(Vector3 pos, ItemType itemType)
    {
        GameObject item = itemPool[(int)itemType].ActivatePoolItem();
        item.transform.position = new Vector3(pos.x, 0.5f, pos.y);
        item.transform.SetParent(items);
        item.GetComponent<ItemBase>().SetUp(itemPool[(int)itemType]);
        //item.GetComponent<Bullet>().Setup(this, bulletPool[(int)type], impactPool[(int)type]);
    }

    public void SpawnDropGun(Vector3 pos, WeaponBase weaponBase)
    {
        GameObject item = itemPool[(int)ItemType.DropGun].ActivatePoolItem();
        //item.transform.position = new Vector3(pos.x, 0.5f, pos.y);
        item.transform.position = pos;
        item.transform.SetParent(items);
        item.GetComponent<ItemBase>().SetUp(itemPool[(int)ItemType.DropGun], weaponBase);
        //item.GetComponent<Bullet>().Setup(this, bulletPool[(int)type], impactPool[(int)type]);
    }

    public void TestSpawn()
    {
        SpawnItem(new Vector3(4, 0.5f, 3), ItemType.DropGun);
        SpawnItem(new Vector3(3, 0.5f, 3), ItemType.DropGun);
        SpawnItem(new Vector3(2, 0.5f, 3), ItemType.DropGun);
        SpawnItem(new Vector3(1, 0.5f, 3), ItemType.DropGun);
        SpawnItem(new Vector3(2, 0.5f, 3), ItemType.MagazineItem);
        SpawnItem(new Vector3(-2, 0.5f, 3), ItemType.HealthItem);
    }
}
