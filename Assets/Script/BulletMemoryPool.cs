using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMemoryPool : MonoBehaviour
{
    [SerializeField]
    private GameObject[] bulletPrefab;      // �Ѿ� ����Ʈ
    private MemoryPool[] bulletPool;        // �Ѿ� ����Ʈ �޸� Ǯ

    [SerializeField]
    private GameObject[] impactPrefab;      // �ǰ� ����Ʈ
    private MemoryPool[] impactPool;        // �ǰ� ����Ʈ �޸� Ǯ


    private void Awake()
    {
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

    public void SpawnBullet(WeaponName type, Vector3 position, Quaternion rotation)
    {
        GameObject bullet = bulletPool[(int)type].ActivatePoolItem();
        bullet.transform.position = position;
        bullet.transform.LookAt(PlayerManager.instance.TargetPosition);
        bullet.GetComponent<Bullet>().Setup(this,bulletPool[(int)type], impactPool[(int)type]);
    }

    public void SpawnImpact(WeaponName type, Vector3 position, Quaternion rotation)
    {
        GameObject impact = impactPool[(int)type].ActivatePoolItem();
        impact.transform.position = position;
        impact.transform.rotation = rotation;
        impact.GetComponent<Impact>().SetUp(impactPool[(int)type]);
    }
}
