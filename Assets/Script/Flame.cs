using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviourPunCallbacks
{
    [SerializeField]
    Bullet.BulletSetting bulletSetting;

    [SerializeField] private ParticleSystem ps;

    private AudioSource audioSource;
    private PhotonView photonView;

    private ParticleSystem.TriggerModule triggerModule;
    private List<ParticleSystem.Particle> particles;

    BoxCollider boxCollider;
    float stayInterval = 0.05f; // 0.05�ʸ��� ����
    bool canCheck = true;
    PlayerManager playerManager;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        boxCollider = GetComponent<BoxCollider>(); // ���� ������Ʈ�� BoxCollider ��������

        if (ps != null)
        {
            triggerModule = ps.trigger;

            // "Enemy" �±װ� �ִ� Collider���� �浹�� �����ϵ��� ����
            triggerModule.SetCollider(0, GameObject.FindWithTag("EnemyFSM").GetComponent<Collider>());
        }
    }

    // �̵� ���� ����
    public void Setup(PlayerManager player, WeaponSetting weaponSetting)
    {
        playerManager = player;
        bulletSetting.weaponName = weaponSetting.WeaponName;
        bulletSetting.bulletDamage = weaponSetting.damage * (1 + weaponSetting.weaponLevel);
        bulletSetting.bulletSpeed = weaponSetting.bulletSpeed;
        bulletSetting.criticalPercent = weaponSetting.critical;
    }

    private void Update()
    {

    }

    private void OnTriggerStay(Collider other)
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
    }

    void ResetCheck()
    {
        canCheck = true;
    }

    // RPC�� ���� ��Ʈ��ũ���� ��Ȱ��ȭ ����ȭ
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        if(isActive) ps.Play();
        boxCollider.enabled = isActive;
        //gameObject.SetActive(isActive);
    }
}
