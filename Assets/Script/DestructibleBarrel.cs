using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField]
    private GameObject destructibleBarrelPieces;

    private bool isDestroyed = false;

    public override void TakeDamage(float damage)
    {
        currentHP -= damage;

        if(currentHP <= 0 && isDestroyed == false)
        {
            isDestroyed = true;

            Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
