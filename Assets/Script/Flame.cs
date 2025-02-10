using Photon.Pun;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Bullet.BulletSetting bulletSetting;

    [SerializeField] private ParticleSystem ps;

    [SerializeField] private PhotonView photonView;

    private ParticleSystem.TriggerModule triggerModule;
    private List<ParticleSystem.Particle> particles;

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // �̹� �浹�� ��ü�� ����

    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;

    [SerializeField] private BoxCollider boxCollider;
    float stayInterval = 0.05f; // 0.05�ʸ��� ����
    bool canCheck = true;
    PlayerManager playerManager;

    private void Awake()
    {
        /*if (ps != null)
        {
            triggerModule = ps.trigger;
            // "Enemy" �±װ� �ִ� Collider���� �浹�� �����ϵ��� ����
            triggerModule.SetCollider(0, GameObject.FindWithTag("EnemyFSM").GetComponent<Collider>());
        }*/
    }

    // �̵� ���� ����
    public void Setup(PlayerManager player, WeaponSetting weaponSetting, BulletMemoryPool BulletMemoryPool, MemoryPool bulletPool, Transform parentTransform)
    {
        playerManager = player;
        bulletSetting.weaponName = weaponSetting.WeaponName;
        bulletSetting.bulletDamage = weaponSetting.damage * (1 + weaponSetting.weaponLevel);
        bulletSetting.bulletSpeed = weaponSetting.bulletSpeed;
        bulletSetting.criticalPercent = weaponSetting.critical;
        hitObjects = new HashSet<GameObject>();

        memoryPool = BulletMemoryPool;
        bulletMemoryPool = bulletPool;

        gameObject.transform.SetParent(parentTransform);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);
    }

    private void Update()
    {
        if (!ps.isPlaying)
        {
            // �Ѿ� ������Ʈ ����
            bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
            photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        }
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
           
        }

        // �浹�� ��ġ�� ����Ʈ ����
        //memoryPool?.SpawnImpact(bulletSetting.weaponName, transform.position, Quaternion.identity);
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (canCheck)
        {
            canCheck = false;

            // BoxCollider�� ���� ���� �߽ɰ� ũ�� ���
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Vector3 worldSize = boxCollider.size * 0.5f; // BoxCollider�� ũ�� ���� (OverlapBox�� Extents ����)

            // Ʈ���� �� ��� ������Ʈ ��������
            Collider[] colliders = Physics.OverlapBox(worldCenter, worldSize, transform.rotation);

            foreach (Collider col in colliders)
            {
                if (!col) continue; // �� üũ

                bool critical = false;
                if (col.transform.CompareTag("EnemyFSM"))
                {
                    if (col.gameObject.name == "Weakness") critical = true;

                    float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

                    if (col.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
                    {
                        // �浹�� ��ġ�� �ؽ�Ʈ ����
                        DamageTextMemoryPool.instance.SpawnText(Damage, critical, transform.position);
                        playerManager.aimHitAnimator.SetTrigger("Show");
                    }
                }
                else if (col.transform.CompareTag("InteractionObject"))
                {
                    col.transform.GetComponent<InteractionObject>().TakeDamage(bulletSetting.bulletDamage);
                }
                else if (col.transform.CompareTag("Shield"))
                {
                    // ���� ���� ���� �߰� ����
                }
            }

            Invoke(nameof(ResetCheck), stayInterval);
        }
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
            
        }
    }*/

    void ResetCheck()
    {
        canCheck = true;
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// ��� Ŭ���̾�Ʈ���� ������Ʈ ��Ȱ��ȭ
    /// </summary>
    [PunRPC]
    public void DeactivateObjectRPC(int viewID)
    {
        bulletMemoryPool.ActiveCount--;
        PhotonView targetPhotonView = PhotonNetwork.GetPhotonView(viewID);

        if (targetPhotonView != null)
        {
            GameObject targetObject = targetPhotonView.gameObject;
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"[MemoryPool] DeactivateObjectRPC: PhotonView with ID {viewID} not found.");
        }
    }
}
