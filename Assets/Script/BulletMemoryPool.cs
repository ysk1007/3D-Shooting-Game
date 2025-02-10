using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class BulletMemoryPool : MonoBehaviourPun
{
    public static BulletMemoryPool instance;

    [SerializeField]
    private GameObject[] bulletPrefab;      // �Ѿ� ������
    private MemoryPool[] bulletPool;        // �Ѿ� �޸� Ǯ

    [SerializeField]
    private GameObject[] impactPrefab;      // �ǰ� ����Ʈ ������
    private MemoryPool[] impactPool;        // �ǰ� ����Ʈ �޸� Ǯ

    [SerializeField] 
    private Transform bulletsParent;        // �Ѿ� ���� �θ� ������Ʈ

    [SerializeField] 
    private Transform impactsParent;        // �ǰ� ����Ʈ ���� �θ� ������Ʈ

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
    /// �޸� Ǯ �ʱ�ȭ
    /// </summary>
    private void InitializePools()
    {
        Debug.Log("�ʱ�ȭ");
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
    /// �Ѿ� ����
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
    /// �ǰ� ����Ʈ ����
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
    /// ��Ʈ��ũ�� ���� �Ѿ� ���� ����ȭ
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
    /// ��Ʈ��ũ�� ���� �ǰ� ����Ʈ ����ȭ
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
