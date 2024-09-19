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
        public float bulletSpeed;       // �Ѿ� �ӵ�
        public float bulletDamage;      // �Ѿ� �����
        public GameObject ImpactObject; // �ǰ� ������
        public GameObject FlashObject;   // �÷��� ������
    }

    [SerializeField]
    BulletSetting bulletSetting;

    [SerializeField]
    private float speed; // �Ѿ� �ӵ�

    [SerializeField]
    private GameObject impactObject; // �浹 �� ������ ����Ʈ ������Ʈ

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

    // �̵� ���� ����
    public void Setup(BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool,Vector3 direction)
    {
        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;
        moveDirection = direction.normalized; // ������ ����ȭ�Ͽ� �ӵ��� �����ϰ� ����
    }

    void Update()
    {
        // ������ �������� �̵�
        transform.position += moveDirection * speed * Time.deltaTime;
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
        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool.SpawnImpact(0, transform.position, Quaternion.identity);

        // �Ѿ� ������Ʈ ����
        bulletMemoryPool.DeactivatePoolItem(this.gameObject);
    }
}
