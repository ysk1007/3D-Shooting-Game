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
        public WeaponName weaponName;   // �Ѿ� �̸�
        public float bulletDamage;      // �Ѿ� �����
        public float criticalPercent;   // ġ��Ÿ ����
        public float bulletSpeed;       // �Ѿ� �ӵ�
        public int bulletPenetration;   // �Ѿ� �����
    }

    private BulletSetting bulletSetting;

    [SerializeField] private ParticleSystem hitEffect;
    private Rigidbody rigidbody;
    private MemoryPool bulletMemoryPool;
    private PhotonView photonView;

    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // �̹� �浹�� ��ü�� ����
    private int penetrationCount;
    private float lifeTimer;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    // �ʱ� ����
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

        if (lifeTimer > 3f) // �Ѿ� ���� �ð� �ʰ� �� ����
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

        // �ǰ� ����Ʈ ����
        BulletMemoryPool.instance.SpawnImpact(bulletSetting.weaponName, hitPoint, Quaternion.identity);

        // �Ѿ� ���� ���� Ȯ��
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
