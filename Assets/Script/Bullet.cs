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
    [Serializable]
    public struct BulletSetting
    {
        public WeaponName weaponName;   // �Ѿ� �̸�
        public float bulletDamage;      // �Ѿ� �����
        public float criticalPercent;   // ġ��Ÿ ����
        public float bulletSpeed;       // �Ѿ� �ӵ�
        public int bulletPenetration; // �Ѿ� �����
    }

    [SerializeField]
    BulletSetting bulletSetting;

    [SerializeField] private ParticleSystem ps;

    private Rigidbody rigidbody;
    private AudioSource audioSource;
    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;
    private MemoryPool impactMemoryPool;
    private PhotonView photonView;

    private ParticleSystem.TriggerModule triggerModule;
    private List<ParticleSystem.Particle> particles;

    PlayerManager playerManager;
    
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // �̹� �浹�� ��ü�� ����
    int PenetrationCount;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();

        if (ps != null)
        {
            triggerModule = ps.trigger;

            // "Enemy" �±װ� �ִ� Collider���� �浹�� �����ϵ��� ����
            triggerModule.SetCollider(0, GameObject.FindWithTag("EnemyFSM").GetComponent<Collider>());
        }
    }

    // �̵� ���� ����
    public void Setup(PlayerManager player, WeaponSetting weaponSetting, BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, MemoryPool impactPool, Transform parentTransform)
    {
        playerManager = player;
        bulletSetting.weaponName = weaponSetting.WeaponName;
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
        bool critical = false;
        if (collision.transform.CompareTag("EnemyFSM"))
        {
            // �̹� �浹�� ��ü���� Ȯ��
            if (hitObjects.Contains(collision.transform.parent.gameObject))
                return;

            hitObjects.Add(collision.transform.parent.gameObject); // �浹�� ��ü ���

            if (collision.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;
            PenetrationCount--;

            if (collision.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
            {
                // �浹�� ��ġ�� �ؽ�Ʈ ����
                DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
                //playerManager.aimHitAnimator.SetTrigger("Show");
            }
        }
        else if (collision.transform.CompareTag("InteractionObject"))
        {
            collision.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
        }
        else if (collision.transform.CompareTag("Shield"))
        {
            PenetrationCount--;
        }

        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool?.SpawnImpact(bulletSetting.weaponName, transform.position, Quaternion.identity);

        if (PenetrationCount >= 0) return;
        // �Ѿ� ������Ʈ ����
        bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }

    // Ʈ���� �浹 ����
    void OnTriggerEnter(Collider other)
    {
        bool critical = false;
        if (other.transform.CompareTag("Enemy"))
        {
            // �̹� �浹�� ��ü���� Ȯ��
            if (hitObjects.Contains(other.transform.parent.gameObject)) return;

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
        else if (other.transform.CompareTag("Shield"))
        {
            PenetrationCount--;
        }

        // �浹�� ��ġ�� ����Ʈ ����
        memoryPool?.SpawnImpact(bulletSetting.weaponName, transform.position, Quaternion.identity);

        if (PenetrationCount >= 0) return;
        // �Ѿ� ������Ʈ ����
        bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }

    void OnParticleTrigger()
    {
        if (particles == null || particles.Count < ps.main.maxParticles)
        {
            particles = new List<ParticleSystem.Particle>(ps.main.maxParticles);
        }

        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for (int i = 0; i < numEnter; i++)
        {
            Debug.Log("��ƼŬ�� Enemy�� �浹��!");
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("��ƼŬ �ø��� �浹");
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
