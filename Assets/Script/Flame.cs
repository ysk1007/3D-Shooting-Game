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

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // 이미 충돌한 객체를 저장

    private BulletMemoryPool memoryPool;
    private MemoryPool bulletMemoryPool;

    [SerializeField] private BoxCollider boxCollider;
    float stayInterval = 0.05f; // 0.05초마다 실행
    bool canCheck = true;
    PlayerManager playerManager;

    private void Awake()
    {
        /*if (ps != null)
        {
            triggerModule = ps.trigger;
            // "Enemy" 태그가 있는 Collider와의 충돌을 감지하도록 설정
            triggerModule.SetCollider(0, GameObject.FindWithTag("EnemyFSM").GetComponent<Collider>());
        }*/
    }

    // 이동 방향 설정
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
            // 총알 오브젝트 제거
            bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
            photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
        }
    }

    // 트리거 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        bool critical = false;
        if (other.transform.CompareTag("Enemy"))
        {
            // 이미 충돌한 객체인지 확인
            if (hitObjects.Contains(other.transform.parent.gameObject)) return;

            hitObjects.Add(other.transform.parent.gameObject); // 충돌한 객체 등록

            if (other.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

            if (other.transform.GetComponentInParent<EnemyFSM>().TakeDamage(Damage))
            {
                // 충돌한 위치에 텍스트 생성
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

        // 충돌한 위치에 이펙트 생성
        //memoryPool?.SpawnImpact(bulletSetting.weaponName, transform.position, Quaternion.identity);
    }

    /*private void OnTriggerStay(Collider other)
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
    }*/

    void ResetCheck()
    {
        canCheck = true;
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 모든 클라이언트에서 오브젝트 비활성화
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
