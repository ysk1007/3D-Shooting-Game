using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKnifeCollider : MonoBehaviour
{
    [SerializeField]
    private ImpactMemoryPool impactMemoryPool;
    [SerializeField]
    private Transform knifeTransform;

    private new Collider collider;
    private float damage;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    public void StartCollider(float damage)
    {
        this.damage = damage;
        collider.enabled = true;

        StartCoroutine("DisablebyTime", 0.1f);
    }

    private IEnumerator DisablebyTime(float time)
    {
        yield return new WaitForSeconds(time);

        collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        impactMemoryPool.SpawnImpact(other, knifeTransform);
        if (other.CompareTag("Enemy"))
        {
            other.GetComponentInParent<EnemyFSM>().TakeDamage(damage);
        }
        else if (other.CompareTag("InteractionObject"))
        {
            other.GetComponent<InteractionObject>().TakeDamage(damage);
        }
    }
}
