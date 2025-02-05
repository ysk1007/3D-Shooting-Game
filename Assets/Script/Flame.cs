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
    float stayInterval = 0.05f; // 0.05초마다 실행
    bool canCheck = true;
    PlayerManager playerManager;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        boxCollider = GetComponent<BoxCollider>(); // 현재 오브젝트의 BoxCollider 가져오기

        if (ps != null)
        {
            triggerModule = ps.trigger;

            // "Enemy" 태그가 있는 Collider와의 충돌을 감지하도록 설정
            triggerModule.SetCollider(0, GameObject.FindWithTag("EnemyFSM").GetComponent<Collider>());
        }
    }

    // 이동 방향 설정
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

            // BoxCollider의 월드 기준 중심과 크기 계산
            Vector3 worldCenter = transform.TransformPoint(boxCollider.center);
            Vector3 worldSize = boxCollider.size * 0.5f; // BoxCollider의 크기 절반 (OverlapBox는 Extents 기준)

            // 트리거 내 모든 오브젝트 가져오기
            Collider[] colliders = Physics.OverlapBox(worldCenter, worldSize, transform.rotation);

            foreach (Collider col in colliders)
            {
                if (!col) continue; // 널 체크

                bool critical = false;
                if (col.transform.CompareTag("EnemyFSM"))
                {
                    if (col.gameObject.name == "Weakness") critical = true;

                    float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

                    if (col.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
                    {
                        // 충돌한 위치에 텍스트 생성
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
                    // 방패 관련 로직 추가 가능
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

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        if(isActive) ps.Play();
        boxCollider.enabled = isActive;
        //gameObject.SetActive(isActive);
    }
}
