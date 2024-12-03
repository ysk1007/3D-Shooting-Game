using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 30f;
    private float projectileDistance = 30;  // 발사체 최대 발사거리
    private float damage = 5;

    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        BulletMove();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Player Hit");
            other.GetComponent<PlayerManager>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }

    private void BulletMove()
    {
        rigidbody.velocity = transform.forward * projectileSpeed;
    }
}
