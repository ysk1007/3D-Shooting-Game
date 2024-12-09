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
        public BulletName BulletName;   // 총알 이름
        public float bulletDamage;      // 총알 대미지
        public float criticalPercent;   // 치명타 배율
        public float bulletSpeed;       // 총알 속도
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

    // 이동 방향 설정
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

    // 충돌 감지
    void OnCollisionEnter(Collision collision)
    {
        // 충돌한 위치에 이펙트 생성
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // 총알 오브젝트 제거
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }

    // 트리거 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        bool critical = false;
        if (other.transform.CompareTag("Enemy"))
        {
            if (other.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

            other.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage);

            // 충돌한 위치에 텍스트 생성
            DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
        }
        else if (other.transform.CompareTag("InteractionObject"))
        {
            other.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
        }

        // 충돌한 위치에 이펙트 생성
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // 총알 오브젝트 제거
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * bulletSetting.bulletSpeed;
    }
}
