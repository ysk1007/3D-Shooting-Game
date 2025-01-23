using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using InfimaGames.LowPolyShooterPack;
using Photon.Pun.Demo.PunBasics;


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
        public int bulletPenetration; // �Ѿ� �����
    }

    [SerializeField]
    BulletSetting bulletSetting;

    private Rigidbody rigidbody;
    private AudioSource audioSource;
    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;
    private MemoryPool impactMemoryPool;
    private PhotonView photonView;

    PlayerManager playerManager;
    
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // �̹� �浹�� ��ü�� ����
    int PenetrationCount;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // �̵� ���� ����
    public void Setup(PlayerManager player, WeaponSetting weaponSetting, BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool, Transform parentTransform)
    {
        playerManager = player;
        bulletSetting.bulletDamage = weaponSetting.damage * (1 + weaponSetting.weaponLevel);
        bulletSetting.bulletSpeed = weaponSetting.bulletSpeed;
        bulletSetting.criticalPercent = weaponSetting.critical;
        PenetrationCount = bulletSetting.bulletPenetration;
        hitObjects = new HashSet<GameObject>();

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
            // �̹� �浹�� ��ü���� Ȯ��
            if (hitObjects.Contains(other.transform.parent.gameObject))
                return;

            hitObjects.Add(other.transform.parent.gameObject); // �浹�� ��ü ���

            if (other.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;
            PenetrationCount--;

            if (other.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
            {
                // �浹�� ��ġ�� �ؽ�Ʈ ����
                DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
                playerManager.aimHitAnimator.SetTrigger("Show");
            }
        }
        else if (other.transform.CompareTag("InteractionObject"))
        {
            other.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
        }

        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool?.SpawnImpact(0, transform.position, Quaternion.identity);

        if (PenetrationCount >= 0) return;
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
