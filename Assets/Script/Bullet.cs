using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;


public class Bullet : MonoBehaviourPunCallbacks
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
    private PhotonView photonView;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // �̵� ���� ����
    public void Setup(WeaponSetting weaponSetting, BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool, Transform parentTransform)
    {
        bulletSetting.bulletDamage = weaponSetting.damage;
        bulletSetting.bulletSpeed = weaponSetting.bulletSpeed;
        bulletSetting.criticalPercent = weaponSetting.critical;

        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;
        impactMemoryPool = impactPool;

        gameObject.transform.SetParent(parentTransform);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);
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

            if(other.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
                // �浹�� ��ġ�� �ؽ�Ʈ ����
                DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
        }
        else if (other.transform.CompareTag("InteractionObject"))
        {
            other.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
        }

        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool?.SpawnImpact(0, transform.position, Quaternion.identity); 

        // �Ѿ� ������Ʈ ����
        bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * bulletSetting.bulletSpeed;
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
