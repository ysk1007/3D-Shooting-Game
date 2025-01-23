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
        public BulletName BulletName;   // 총알 이름
        public float bulletDamage;      // 총알 대미지
        public float criticalPercent;   // 치명타 배율
        public float bulletSpeed;       // 총알 속도
        public int bulletPenetration; // 총알 관통력
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
    
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // 이미 충돌한 객체를 저장
    int PenetrationCount;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // 이동 방향 설정
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
        bool critical = false;
        if (other.transform.CompareTag("Enemy"))
        {
            // 이미 충돌한 객체인지 확인
            if (hitObjects.Contains(other.transform.parent.gameObject))
                return;

            hitObjects.Add(other.transform.parent.gameObject); // 충돌한 객체 등록

            if (other.gameObject.name == "Weakness") critical = true;

            float Damage = critical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;
            PenetrationCount--;

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

        // 충돌한 위치에 이펙트 생성
        memoryPool?.SpawnImpact(0, transform.position, Quaternion.identity);

        if (PenetrationCount >= 0) return;
        // 총알 오브젝트 제거
        bulletMemoryPool?.DeactivatePoolItem(this.gameObject);
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, false);
    }

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * bulletSetting.bulletSpeed;
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
