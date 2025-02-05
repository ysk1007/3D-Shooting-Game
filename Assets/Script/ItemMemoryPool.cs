using Photon.Pun;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum ItemType { HealthItem = 0, MagazineItem = 1, DropGun = 2, Coin = 3, Exp = 4}

public class ItemMemoryPool : MonoBehaviour
{
    public static ItemMemoryPool instance;

    [SerializeField] private GameObject[] itemPrefab;      // 아이템 프리팹

    private MemoryPool[] itemPool;                         // 아이템 메모리 풀

    [SerializeField] private Transform items;              // 관리할 부모 오브젝트

    [SerializeField] private PhotonView photonView;

    private void Awake()
    {
        instance = this;
        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        itemPool = new MemoryPool[itemPrefab.Length];
        for (int i = 0; i < itemPrefab.Length; ++i)
        {
            itemPool[i] = new MemoryPool(itemPrefab[i]);
        }
    }

    private void Start()
    {
        TestSpawn();
    }

    public void SpawnItem(Vector3 pos, ItemType itemType)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject item = itemPool[(int)itemType].ActivatePoolItem();
        item.transform.position = new Vector3(pos.x, 0.5f, pos.z);
        item.transform.SetParent(items);
        //item.GetComponent<ItemBase>().SetUp(itemPool[(int)itemType].GetComponent<PhotonView>().ViewID);
        item.GetComponent<PhotonView>().RPC("ItemSetUp", RpcTarget.AllBuffered, photonView.ViewID, null);//itemPool[(int)itemType].GetComponent<PhotonView>().ViewID);
    }

    public void SpawnDropGun(Vector3 pos, WeaponBase weaponBase)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject item = itemPool[(int)ItemType.DropGun].ActivatePoolItem();
        //item.transform.position = new Vector3(pos.x, 0.5f, pos.y);
        item.transform.position = pos;
        item.transform.SetParent(items);
        //item.GetComponent<ItemBase>().SetUp(itemPool[(int)ItemType.DropGun], weaponBase);
        string weaponBaseJson = JsonUtility.ToJson(weaponBase.ToData());
        item.GetComponent<PhotonView>().RPC("ItemSetUp", RpcTarget.AllBuffered, photonView.ViewID, weaponBaseJson);
    }

    public void SpawnDropGun(Vector3 pos, WeaponBase weaponBase, WeaponSetting weaponSetting)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject item = itemPool[(int)ItemType.DropGun].ActivatePoolItem();
        //item.transform.position = new Vector3(pos.x, 0.5f, pos.y);
        item.transform.position = pos;
        item.transform.SetParent(items);

        weaponBase.WeaponSetting = weaponSetting;
        string weaponBaseJson = JsonUtility.ToJson(weaponBase.ToData());
        item.GetComponent<PhotonView>().RPC("ItemSetUp", RpcTarget.AllBuffered, photonView.ViewID, weaponBaseJson);
    }

    public void TestSpawn()
    {
        SpawnDropGun(new Vector3(7, 0.5f, 3), GunMemoryPool.instance.Weapons[0], GunMemoryPool.instance.Weapons[0].WeaponSetting);
        SpawnDropGun(new Vector3(5, 0.5f, 3), GunMemoryPool.instance.Weapons[1], GunMemoryPool.instance.Weapons[1].WeaponSetting);
        SpawnDropGun(new Vector3(3, 0.5f, 3), GunMemoryPool.instance.Weapons[2], GunMemoryPool.instance.Weapons[2].WeaponSetting);
        SpawnDropGun(new Vector3(1, 0.5f, 3), GunMemoryPool.instance.Weapons[3], GunMemoryPool.instance.Weapons[3].WeaponSetting);
        SpawnDropGun(new Vector3(-1, 0.5f, 3), GunMemoryPool.instance.Weapons[4], GunMemoryPool.instance.Weapons[4].WeaponSetting);
        SpawnDropGun(new Vector3(-3, 0.5f, 3), GunMemoryPool.instance.Weapons[5], GunMemoryPool.instance.Weapons[5].WeaponSetting);
        //SpawnItem(new Vector3(2, 0.5f, 3), ItemType.MagazineItem);
        //SpawnItem(new Vector3(-2, 0.5f, 3), ItemType.HealthItem);
    }

    public void DeactivateItem(GameObject Item)
    {
        itemPool[(int)Item.GetComponent<ItemBase>().itemType]?.DeactivatePoolItem(Item);
        Item.GetComponent<PhotonView>().RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }
}
