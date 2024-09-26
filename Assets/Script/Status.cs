using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HPEvent : UnityEngine.Events.UnityEvent<float, float> { }
public class Status : MonoBehaviour
{
    [HideInInspector]
    public HPEvent onHPEvent = new HPEvent();

    [Header("Walk, Run Speed")]
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    [Header("HP")]
    [SerializeField]
    private float maxHP = 100;
    private float currentHP;

    [Header("Attack")]
    [SerializeField]
    private float damage;

    public float WalkSpeed => walkSpeed;
    public float RunSpeed => runSpeed;

    public float CurrentHP => currentHP;
    public float MaxHP => maxHP;

    public float Damage => damage;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public bool DecreaseHP(float damage)
    {
        float previousHP = currentHP;

        currentHP = CurrentHP - damage > 0 ? currentHP - damage : 0;

        onHPEvent.Invoke(previousHP, currentHP);

        if(currentHP == 0)
        {
            return true;
        }

        return false;
    }
}
