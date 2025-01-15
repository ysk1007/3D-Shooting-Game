using Photon.Pun;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMemoryPool : MonoBehaviour
{
    public static BulletMemoryPool instance;

    [SerializeField]
    private GameObject[] bulletPrefab;      // 총알 이펙트
    private MemoryPool[] bulletPool;        // 총알 이펙트 메모리 풀

    [SerializeField]
    private GameObject[] impactPrefab;      // 피격 이펙트
    private MemoryPool[] impactPool;        // 피격 이펙트 메모리 풀

    [SerializeField] private Transform bullets; // 관리할 부모 오브젝트
    [SerializeField] private Transform impacts; // 관리할 부모 오브젝트

    private void Awake()
    {
        instance = this;
        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        bulletPool = new MemoryPool[bulletPrefab.Length];
        for (int i = 0; i < bulletPrefab.Length; ++i)
        {
            bulletPool[i] = new MemoryPool(bulletPrefab[i]);
        }

        // 피격 이펙트가 여러 종류이면 종류별로 memoryPool 생성
        impactPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; ++i)
        {
            impactPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    public void SpawnBullet(PlayerManager player, WeaponSetting weaponSetting, Vector3 position, Quaternion rotation, Vector3 LookAt)
    {
        GameObject bullet = bulletPool[(int)weaponSetting.WeaponName].ActivatePoolItem();
        bullet.transform.SetParent(bullets);
        bullet.transform.position = position;
        bullet.transform.LookAt(LookAt);
        bullet.GetComponent<Bullet>().Setup(player, weaponSetting, this, bulletPool[(int)weaponSetting.WeaponName], impactPool[(int)weaponSetting.WeaponName], bullets);
    }

    public void SpawnImpact(WeaponName type, Vector3 position, Quaternion rotation)
    {
        GameObject impact = impactPool[(int)type].ActivatePoolItem();
        impact.transform.SetParent(impacts);
        impact.transform.position = position;
        impact.transform.rotation = rotation;
        impact.GetComponent<Impact>().SetUp(impactPool[(int)type]);
    }
}
