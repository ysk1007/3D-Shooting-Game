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

    // 이동 방향 설정
    public void Setup(BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool,Vector3 direction)
    {
        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;
        movementTransform.MoveTo(direction.normalized); // 방향을 정규화하여 속도를 일정하게 유지
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
        if (other.transform.CompareTag("Enemy"))
        {
            other.transform.GetComponent<EnemyFSM>().TakeDamage(bulletSetting.bulletDamage);
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
}
