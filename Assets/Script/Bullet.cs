using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum BulletName { AssaultRifle = 0 }

    [Serializable]
    public struct BulletSetting
    {
        public BulletName BulletName;   // �Ѿ� �̸�
        public float bulletDamage;      // �Ѿ� �����
    }

    [SerializeField]
    BulletSetting bulletSetting;

    [SerializeField]
    private MovementTransform movementTransform;

    private Rigidbody rigidbody3D;
    private AudioSource audioSource;
    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;
    private MemoryPool impactMemoryPool;

    // �̵� ���� ����
    public void Setup(BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool,Vector3 direction)
    {
        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;
        movementTransform.MoveTo(direction.normalized); // ������ ����ȭ�Ͽ� �ӵ��� �����ϰ� ����
    }

    // �浹 ����
    void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // �Ѿ� ������Ʈ ����
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }

    // Ʈ���� �浹 ����
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<EnemyFSM>().TakeDamage(bulletSetting.bulletDamage);
        }
        else if (other.transform.CompareTag("InteractionObject"))
        {
            other.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
        }

        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // �Ѿ� ������Ʈ ����
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }
}
