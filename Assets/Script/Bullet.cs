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
        public float criticalPercent;   // ġ��Ÿ ����
        public float bulletSpeed;       // �Ѿ� �ӵ�
    }

    [SerializeField]
    BulletSetting bulletSetting;

    private Rigidbody rigidbody;
    private AudioSource audioSource;
    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;
    private MemoryPool impactMemoryPool;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // �̵� ���� ����
    public void Setup(WeaponSetting weaponSetting, BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool)
    {
        bulletSetting.bulletDamage = weaponSetting.damage;
        bulletSetting.bulletSpeed = weaponSetting.bulletSpeed;
        bulletSetting.criticalPercent = weaponSetting.critical;

        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;
    }

    private void Update()
    {
        BulletMove();
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
        bool critical = false;
        if (other.transform.CompareTag("Enemy"))
        {
            if (other.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

            other.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage);

            // �浹�� ��ġ�� �ؽ�Ʈ ����
            DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
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

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * bulletSetting.bulletSpeed;
    }
}
