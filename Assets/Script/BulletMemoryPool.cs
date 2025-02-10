using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BulletMemoryPool : MonoBehaviourPun
{
    public static BulletMemoryPool instance;

    [SerializeField]
    private GameObject[] bulletPrefab;      // 총알 프리팹
    private MemoryPool[] bulletPool;        // 총알 메모리 풀

    [SerializeField]
    private GameObject[] impactPrefab;      // 피격 이펙트 프리팹
    private MemoryPool[] impactPool;        // 피격 이펙트 메모리 풀

    [SerializeField] 
    private Transform bulletsParent;        // 총알 관리 부모 오브젝트

    [SerializeField] 
    private Transform impactsParent;        // 피격 이펙트 관리 부모 오브젝트

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializePools();
    }

    /// <summary>
    /// 메모리 풀 초기화
    /// </summary>
    private void InitializePools()
    {
        Debug.Log("초기화");
        bulletPool = new MemoryPool[bulletPrefab.Length];
        for (int i = 0; i < bulletPrefab.Length; i++)
        {
            bulletPool[i] = new MemoryPool(bulletPrefab[i]);
        }

        impactPool = new MemoryPool[impactPrefab.Length];
        for (int i = 0; i < impactPrefab.Length; i++)
        {
            impactPool[i] = new MemoryPool(impactPrefab[i]);
        }
    }

    /// <summary>
    /// 총알 생성
    /// </summary>
    public void SpawnBullet(PlayerManager player, WeaponSetting weaponSetting, Vector3 position, Quaternion rotation, Vector3 target)
    {
        int weaponIndex = (int)weaponSetting.WeaponName;
        if (weaponIndex >= bulletPool.Length) return;

        GameObject bullet = bulletPool[weaponIndex].ActivatePoolItem(position);
        if (bullet == null) return;
        bullet.transform.SetParent(bulletsParent);
        bullet.transform.position = position;
        bullet.transform.LookAt(target);

        if (bullet.TryGetComponent(out Bullet bulletComponent))
        {
            bulletComponent.Setup(new Bullet.BulletSetting
            {
                weaponName = weaponSetting.WeaponName,
                bulletDamage = weaponSetting.damage * (1 + weaponSetting.weaponLevel),
                bulletSpeed = weaponSetting.bulletSpeed,
                criticalPercent = weaponSetting.critical,
                bulletPenetration = weaponSetting.bulletPenetration,
            }, bulletPool[weaponIndex], bulletsParent);
        }
        else if (bullet.TryGetComponent(out Flame flameComponent))
        {
            flameComponent.Setup(player, weaponSetting, this, bulletPool[weaponIndex], bulletsParent);
        }

        photonView.RPC(nameof(SyncSpawnBullet), RpcTarget.OthersBuffered, position, rotation);
    }

    /// <summary>
    /// 피격 이펙트 생성
    /// </summary>
    public void SpawnImpact(WeaponName type, Vector3 position, Quaternion rotation)
    {
        int typeIndex = (int)type;
        if (typeIndex >= impactPool.Length) return;

        GameObject impact = impactPool[typeIndex].ActivatePoolItem(position);
        impact.transform.SetParent(impactsParent);
        impact.transform.position = position;
        impact.transform.rotation = rotation;

        if (impact.TryGetComponent(out Impact impactComponent))
        {
            impactComponent.SetUp(impactPool[typeIndex]);
        }

        photonView.RPC("SyncSpawnImpact", RpcTarget.OthersBuffered, typeIndex, position, rotation);
    }

    /// <summary>
    /// 네트워크를 통한 총알 생성 동기화
    /// </summary>
    [PunRPC]
    private void SyncSpawnBullet(Vector3 position, Quaternion rotation)
    {
        int randomIndex = Random.Range(0, bulletPool.Length);
        GameObject bullet = bulletPool[randomIndex].ActivatePoolItem(position);
        bullet.transform.SetParent(bulletsParent);
        bullet.transform.position = position;
        bullet.transform.rotation = rotation;
    }

    /// <summary>
    /// 네트워크를 통한 피격 이펙트 동기화
    /// </summary>
    [PunRPC]
    private void SyncSpawnImpact(int typeIndex, Vector3 position, Quaternion rotation)
    {
        if (typeIndex >= impactPool.Length) return;

        GameObject impact = impactPool[typeIndex].ActivatePoolItem(position);
        impact.transform.SetParent(impactsParent);
        impact.transform.position = position;
        impact.transform.rotation = rotation;
    }
}
