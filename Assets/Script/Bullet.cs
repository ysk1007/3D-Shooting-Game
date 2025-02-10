using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [Serializable]
    public struct BulletSetting
    {
        public WeaponName weaponName;   // 총알 이름
        public float bulletDamage;      // 총알 대미지
        public float criticalPercent;   // 치명타 배율
        public float bulletSpeed;       // 총알 속도
        public int bulletPenetration;   // 총알 관통력
    }

    private BulletSetting bulletSetting;

    [SerializeField] private ParticleSystem hitEffect;
    private Rigidbody rigidbody;
    private MemoryPool bulletMemoryPool;
    private PhotonView photonView;

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // 이미 충돌한 객체를 저장
    private int penetrationCount;
    private float lifeTimer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // 초기 설정
    public void Setup(BulletSetting setting, MemoryPool bulletPool, Transform parentTransform)
    {
        bulletSetting = setting;
        penetrationCount = bulletSetting.bulletPenetration;
        hitObjects.Clear();
        lifeTimer = 0f;

        bulletMemoryPool = bulletPool;
        transform.SetParent(parentTransform);

        photonView.RPC(nameof(ActivateObjectRPC), RpcTarget.AllBuffered, true);
    }

    private void Update()
    {
        BulletMove();
        lifeTimer += Time.deltaTime;

        if (lifeTimer > 3f) // 총알 유지 시간 초과 시 제거
        {
            DeactivateBullet();
        }
    }

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * bulletSetting.bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject, collision.contacts[0].point);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject, transform.position);
    }

    private void HandleHit(GameObject hitObject, Vector3 hitPoint)
    {
        bool isCritical = hitObject.name == "Weakness";
        float damage = isCritical ? bulletSetting.bulletDamage * bulletSetting.criticalPercent : bulletSetting.bulletDamage;

        if (hitObject.CompareTag("EnemyFSM") || hitObject.CompareTag("Enemy"))
        {
            if (!hitObjects.Add(hitObject.transform.parent.gameObject)) return;

            if (hitObject.GetComponentInParent<EnemyFSM>().TakeDamage(damage))
            {
                DamageTextMemoryPool.instance.SpawnText(damage, isCritical, hitPoint);
            }
        }
        else if (hitObject.CompareTag("InteractionObject"))
        {
            hitObject.GetComponent<InteractionObject>()?.TakeDamage(bulletSetting.bulletDamage);
        }
        else if (hitObject.CompareTag("Shield"))
        {
            penetrationCount--;
        }

        // 피격 이펙트 생성
        BulletMemoryPool.instance.SpawnImpact(bulletSetting.weaponName, hitPoint, Quaternion.identity);

        // 총알 삭제 여부 확인
        if (--penetrationCount < 0)
        {
            DeactivateBullet();
        }
    }

    private void DeactivateBullet()
    {
        bulletMemoryPool?.DeactivatePoolItem(gameObject);
        photonView.RPC(nameof(ActivateObjectRPC), RpcTarget.AllBuffered, false);
    }

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
