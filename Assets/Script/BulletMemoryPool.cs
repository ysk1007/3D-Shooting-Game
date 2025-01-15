using Photon.Pun;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMemoryPool : MonoBehaviour
{
    public static BulletMemoryPool instance;

    [SerializeField]
    private GameObject[] bulletPrefab;      // �Ѿ� ����Ʈ
    private MemoryPool[] bulletPool;        // �Ѿ� ����Ʈ �޸� Ǯ

    [SerializeField]
    private GameObject[] impactPrefab;      // �ǰ� ����Ʈ
    private MemoryPool[] impactPool;        // �ǰ� ����Ʈ �޸� Ǯ

    [SerializeField] private Transform bullets; // ������ �θ� ������Ʈ
    [SerializeField] private Transform impacts; // ������ �θ� ������Ʈ

    private void Awake()
    {
        instance = this;
        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
        bulletPool = new MemoryPool[bulletPrefab.Length];
        for (int i = 0; i < bulletPrefab.Length; ++i)
        {
            bulletPool[i] = new MemoryPool(bulletPrefab[i]);
        }

        // �ǰ� ����Ʈ�� ���� �����̸� �������� memoryPool ����
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
