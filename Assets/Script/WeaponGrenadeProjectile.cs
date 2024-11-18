using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGrenadeProjectile : MonoBehaviour
{
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private float explosionRadius = 10.0f;
    [SerializeField]
    private float explosionForce = 500.0f;
    [SerializeField]
    private float throwForce = 1000.0f;

    private float explosionDamage;
    private new Rigidbody rigidbody;
    
    public void Setup(float damage, Vector3 rotation)
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(rotation * throwForce);

        explosionDamage = damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ����Ʈ ����
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        // ���� ������ �ִ� ��� ������Ʈ�� Collider ������ �޾ƿ� ���� ȿ�� ó��
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // ���� ������ �ε��� ������Ʈ�� �÷��̾��� �� ó��
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage * 0.2f);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� �� ĳ������ �� ó��
            EnemyFSM enemy = hit.GetComponent<EnemyFSM>();
            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
                continue;
            }

            // ���� ������ �ε��� ������Ʈ�� ��ȣ�ۿ� ������Ʈ�̸� TakeDamage()�� ���ظ� ��
            InteractionObject interaction = hit.GetComponent<InteractionObject>();
            if (interaction != null)
            {
                interaction.TakeDamage(explosionDamage);
            }

            // �߷��� ������ �ִ� ������Ʈ�̸� ���� �޾� �з�������
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // ����ź ������Ʈ ����
        Destroy(gameObject);
    }
}
