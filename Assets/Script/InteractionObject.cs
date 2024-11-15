using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    [Header("InteractionObject")]
    [SerializeField]
    protected float maxHP = 100;
    protected float currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public abstract void TakeDamage(float damage);
}
