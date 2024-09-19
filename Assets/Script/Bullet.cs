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
        public float bulletSpeed;       // 총알 속도
        public float bulletDamage;      // 총알 대미지
        public GameObject ImpactObject; // 피격 프리팹
        public GameObject FlashObject;   // 플래시 프리팹
    }

    [SerializeField]
    BulletSetting bulletSetting;

    [SerializeField]
    private float speed; // 총알 속도

    [SerializeField]
    private GameObject impactObject; // 충돌 시 생성될 이펙트 오브젝트

    [SerializeField]
    private Vector3 moveDirection;

    private Rigidbody rigidbody3D;
    private AudioSource audioSource;
    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;
    private MemoryPool impactMemoryPool;

    private void OnEnable()
    {
        speed = bulletSetting.bulletSpeed;
    }

    // 이동 방향 설정
    public void Setup(BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool,Vector3 direction)
    {
        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;
        moveDirection = direction.normalized; // 방향을 정규화하여 속도를 일정하게 유지
    }

    void Update()
    {
        // 설정된 방향으로 이동
        transform.position += moveDirection * speed * Time.deltaTime;
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
        // 충돌한 위치에 이펙트 생성
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // 총알 오브젝트 제거
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }
}
